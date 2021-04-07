using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hortensia.Core.Threads
{
    /// <summary>
    /// Thanks to Stump.
    /// </summary>
    internal static class TaskHelper
    {
        /// <summary>
        ///   Gets the TaskScheduler instance that should be used to schedule tasks.
        /// </summary>
        public static TaskScheduler GetTargetScheduler(this TaskFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            return factory.Scheduler ?? TaskScheduler.Current;
        }

        /// <summary>
        ///   Creates a Task that will complete after the specified delay.
        /// </summary>
        /// <param name = "factory">The TaskFactory.</param>
        /// <param name = "millisecondsDelay">The delay after which the Task should transition to RanToCompletion.</param>
        /// <param name = "cancellationToken">The cancellation token that can be used to cancel the timed task.</param>
        /// <returns>A Task that will be completed after the specified duration and that's cancelable with the specified token.</returns>
        public static Task StartNewDelayed(this TaskFactory factory, int millisecondsDelay,
                                           CancellationToken cancellationToken)
        {
            // Validate arguments
            if (factory == null) throw new ArgumentNullException("factory");
            if (millisecondsDelay < 0) throw new ArgumentOutOfRangeException("millisecondsDelay");

            // Create the timed task
            var tcs = new TaskCompletionSource<object>(factory.CreationOptions);
            CancellationTokenRegistration[] ctr = { default };

            // Create the timer but don't start it yet.  If we start it now,
            // it might fire before ctr has been set to the right registration.
            var timer = new Timer(self =>
            {
                // Clean up both the cancellation token and the timer, and try to transition to completed
                ctr[0].Dispose();
                ((Timer)self).Dispose();
                tcs.TrySetResult(null);
            });

            // Register with the cancellation token.
            if (cancellationToken.CanBeCanceled)
                // When cancellation occurs, cancel the timer and try to transition to canceled.
                // There could be a race, but it's benign.
                ctr[0] = cancellationToken.Register(() =>
                {
                    timer.Dispose();
                    tcs.TrySetCanceled();
                });

            // Start the timer and hand back the task...
            timer.Change(millisecondsDelay, Timeout.Infinite);
            return tcs.Task;
        }

        /// <summary>
        ///   Creates and schedules a task for execution after the specified time delay.
        /// </summary>
        /// <param name = "factory">The factory to use to create the task.</param>
        /// <param name = "millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name = "action">The delegate executed by the task.</param>
        /// <param name = "cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <param name = "creationOptions">Options that control the task's behavior.</param>
        /// <param name = "scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action action,
            CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (millisecondsDelay < 0) throw new ArgumentOutOfRangeException("millisecondsDelay");
            if (action == null) throw new ArgumentNullException("action");
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            return factory
                .StartNewDelayed(millisecondsDelay, cancellationToken)
                .ContinueWith(_ => action(), cancellationToken, TaskContinuationOptions.OnlyOnRanToCompletion, scheduler);
        }

        /// <summary>
        ///   Creates and schedules a task for execution after the specified time delay.
        /// </summary>
        /// <param name = "factory">The factory to use to create the task.</param>
        /// <param name = "millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name = "action">The delegate executed by the task.</param>
        /// <param name = "state">An object provided to the delegate.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action<object> action, object state)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            return factory.StartNewDelayed(millisecondsDelay, action, state, factory.CancellationToken,
                                   factory.CreationOptions, factory.GetTargetScheduler());
        }

        /// <summary>
        ///   Creates and schedules a task for execution after the specified time delay.
        /// </summary>
        /// <param name = "factory">The factory to use to create the task.</param>
        /// <param name = "millisecondsDelay">The delay after which the task will be scheduled.</param>
        /// <param name = "action">The delegate executed by the task.</param>
        /// <param name = "state">An object provided to the delegate.</param>
        /// <param name = "cancellationToken">The cancellation token to assign to the created Task.</param>
        /// <param name = "creationOptions">Options that control the task's behavior.</param>
        /// <param name = "scheduler">The scheduler to which the Task will be scheduled.</param>
        /// <returns>The created Task.</returns>
        public static Task StartNewDelayed(
            this TaskFactory factory,
            int millisecondsDelay, Action<object> action, object state,
            CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (millisecondsDelay < 0) throw new ArgumentOutOfRangeException("millisecondsDelay");
            if (action == null) throw new ArgumentNullException("action");
            if (scheduler == null) throw new ArgumentNullException("scheduler");

            // Create the task that will be returned; workaround for no ContinueWith(..., state) overload.
            var result = new TaskCompletionSource<object>(state);

            // Delay a continuation to run the action
            factory
                .StartNewDelayed(millisecondsDelay, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsCanceled) result.TrySetCanceled();
                    else
                        try
                        {
                            action(state);
                            result.TrySetResult(null);
                        }
                        catch (Exception exc)
                        {
                            result.TrySetException(exc);
                        }
                }, scheduler);

            // Return the task
            return result.Task;
        }

    }
}
