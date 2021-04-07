using System;

namespace Hortensia.Core.Threads.Timers
{
    public class TimerConfigurationEntry
    {
        public double Interval { get; set; }
        public TimerTypeEntry Type { get; set; }
        public bool AutoReset { get; set; } = true;

        public int GetTime()
        {
            int fixedInterval = Type switch
            {
                TimerTypeEntry.Seconds => Convert.ToInt32(Interval * 1000),
                TimerTypeEntry.Minutes => Convert.ToInt32(Interval * 60000),
                TimerTypeEntry.Hours => Convert.ToInt32(Interval * 3600000),
                TimerTypeEntry.Days => Convert.ToInt32(Interval * 86400000),
                _ => 0,
            };

            return fixedInterval;
        }
    }
}
