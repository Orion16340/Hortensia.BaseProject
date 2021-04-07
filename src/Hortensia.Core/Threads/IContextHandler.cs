using Hortensia.Core.Threads.Callback;
using Hortensia.Core.Threads.Timers;
using System;

namespace Hortensia.Core.Threads
{
    /// <summary>
    /// <see cref="IContextHandler"/> represents a set of instructions to be performed by one <see cref="Thread"/> 
    /// at a time and allows <see cref="Instruction"/> to be dispatched.
    /// </summary>
    /// <remarks>Thank's to WCell Project for this great idea and Stump.</remarks>
    public interface IContextHandler
    {
        bool IsInContext { get; }

        void Start();

        IExecutable ExecutePeriodically(Action method, TimerConfigurationEntry config);

        IExecutable ExecuteDelayed(Action method, TimerConfigurationEntry config);

        /// <summary>
        /// Executes action instantly, if in context.
        /// Enqueues a Message to execute it later, if not in context.
        /// </summary>
        void ExecuteInContext(Action action);

    }
}
