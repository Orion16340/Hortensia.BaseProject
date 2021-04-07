using System;

namespace Hortensia.Core.Threads.Timers
{
    public class TimerEntry : ITimerEntry
    {
        private int m_interval;
        private int m_delay;
        private bool m_alreadyCalled;

        public bool Enabled { get; set; }

        public DateTime NextTick { get; private set; }

        public int Interval
        {
            get => m_interval;
            set
            {
                if (value != default)
                    NextTick = NextTick - TimeSpan.FromMilliseconds(m_interval) + TimeSpan.FromMilliseconds(value);
                m_interval = value;
            }
        }

        public int Delay
        {
            get => m_delay;
            set
            {
                if (!m_alreadyCalled && Enabled && value != default)
                    NextTick = NextTick - TimeSpan.FromMilliseconds(m_delay) + TimeSpan.FromMilliseconds(value);
                m_delay = value;
            }
        }

        /// <summary>
        /// <see cref="TimerEntry"/> for delayed call.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        public TimerEntry(int delay, int interval)
        {
            m_delay = delay;
            Interval = interval;
        }

        /// <summary>
        /// <see cref="TimerEntry"/> for periodic calls.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        public TimerEntry(int interval) : this(interval, interval) { }


        public void Start()
        {
            NextTick = DateTime.Now + TimeSpan.FromMilliseconds(m_delay);
            Enabled = true;
        }

        public void UpdateTimerEntry()
        {
            if (Interval < 0)
                Enabled = false;
            else
                NextTick = DateTime.Now + TimeSpan.FromMilliseconds(Interval);

            m_alreadyCalled = true;
        }
    }
}
