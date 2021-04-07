using System.Collections.Generic;

namespace Hortensia.Threads.Sync.Queue
{
    public class SyncQueue<TEntity>
    {
        private readonly Queue<TEntity> m_queue;
        private readonly object m_sync;


        public SyncQueue()
        {
            this.m_queue = new Queue<TEntity>();
            this.m_sync = new object();
        }


        public void Enqueue(TEntity item)
        {
            lock (m_sync)
                m_queue.Enqueue(item);
        }

        public bool TryDequeue(out TEntity item)
        {
            lock (m_sync)
                return m_queue.TryDequeue(out item);
        }

        public bool Contains(TEntity entity)
        {
            lock (m_sync)
                return m_queue.Contains(entity);
        }
    }
}
