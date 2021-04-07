using System;
using System.Threading;

namespace Hortensia.Core.Threads
{
    public sealed class AsyncRandom : Random
    {
        private static int _incrementer;
        public AsyncRandom() : base(Environment.TickCount + Thread.CurrentThread.ManagedThreadId + _incrementer)
             => Interlocked.Increment(ref _incrementer);

        public AsyncRandom(int seed) : base(seed) { }

        public double NextDouble(double min, double max) => this.NextDouble() * (max - min) + min;
    }
}
