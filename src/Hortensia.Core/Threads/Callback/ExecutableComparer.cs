using System.Collections.Generic;

namespace Hortensia.Core.Threads.Callback
{
    internal class ExecutableComparer : IComparer<IExecutable>
    {
        public int Compare(IExecutable x, IExecutable y)
             => x.TimerEntry.NextTick.CompareTo(y.TimerEntry.NextTick);
    }
}
