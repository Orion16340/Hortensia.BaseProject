using Hortensia.Core.Threads.Timers;
using System;

namespace Hortensia.Core.Threads.Callback
{
    public class Instruction : IExecutable
    {
        private readonly Action m_callback;

        public ITimerEntry TimerEntry { get; private set; }

        public bool Enabled
        {
            get => this.TimerEntry.Enabled;
            set => this.TimerEntry.Enabled = value;
        }

        public bool IsDisposed { get; private set; }


        /// <summary>
        /// <see cref="Instruction"/> performs the single call of <see cref="Action"/>.
        /// </summary>
        /// <param name="callback"></param>
        public Instruction(Action callback)
            => this.m_callback = callback;

        /// <summary>
        /// <see cref="Instruction"/> performs the delayed call of <see cref="Action"/>.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="delayMillis"></param>
        public Instruction(Action callback, int delayMillis)
        {
            this.m_callback = callback;
            this.TimerEntry = new TimerEntry(delayMillis);
        }

        /// <summary>
        /// <see cref="Instruction"/> performs the periodic call of <see cref="Action"/>.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="delayMillis"></param>
        /// <param name="intervalMillis"></param>
        public Instruction(Action callback, int delayMillis, int intervalMillis = default)
        {
            this.m_callback = callback;
            this.TimerEntry = new TimerEntry(delayMillis, intervalMillis);
        }


        public void Execute()
        {
            m_callback?.Invoke();

            if (TimerEntry?.Interval == default(int)) /* Single delayed action */
                IsDisposed = true;

            else TimerEntry?.UpdateTimerEntry();
        }
    }
}
