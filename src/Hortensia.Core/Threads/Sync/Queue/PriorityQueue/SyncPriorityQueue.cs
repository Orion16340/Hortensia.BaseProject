using Medallion.Collections;
using System.Collections.Generic;

namespace Hortensia.Threads.Sync.Queue.PriorityQueue
{
    public class SyncPriorityQueue<TEntity, TComparer> where TComparer : IComparer<TEntity>, new()
    {
        public PriorityQueue<TEntity> Data { get; }

        public SyncPriorityQueue() { this.Data = new PriorityQueue<TEntity>(new TComparer()); }

    }
}