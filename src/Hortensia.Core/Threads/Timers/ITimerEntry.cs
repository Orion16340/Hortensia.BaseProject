using System;

namespace Hortensia.Core.Threads.Timers
{
    public interface ITimerEntry
    {
        int Delay { get; set; }

        bool Enabled { get; set; }

        int Interval { get; set; }

        DateTime NextTick { get; }

        void Start();

        void UpdateTimerEntry();
    }
}
