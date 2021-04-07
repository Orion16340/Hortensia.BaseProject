using Hortensia.Core.Threads.Timers;

namespace Hortensia.Core.Threads.Callback
{
    public interface IExecutable
    {

        ITimerEntry TimerEntry { get; }

        bool Enabled { get; set; }

        bool IsDisposed { get; }

        void Execute();

    }
}
