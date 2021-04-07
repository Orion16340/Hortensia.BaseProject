using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hortensia.Core.Threads.Sync.List
{
    /// <summary>
    /// <see cref="SyncList{TEntity}"/> represents a thread-safe list.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SyncList<TEntity> : IEnumerable<TEntity>
    {
        private readonly List<TEntity> m_storage;
        private readonly ReaderWriterLockSlim m_sync = new ReaderWriterLockSlim();
        private int m_count;

        public int Count => Thread.VolatileRead(ref m_count);

        public SyncList() { this.m_storage = new List<TEntity>(); }

        public TEntity this[int index]
        {
            get
            {
                m_sync.EnterReadLock();

                try
                { return m_storage[index]; }

                finally
                { m_sync.ExitReadLock(); }
            }
            set
            {
                m_sync.EnterWriteLock();

                try
                { m_storage[index] = value; }
                finally
                { m_sync.ExitWriteLock(); }
            }
        }

        public void Add(TEntity item)
        {
            m_sync.EnterWriteLock();

            try
            {
                m_storage.Add(item);
                Interlocked.Increment(ref m_count);
            }

            finally
            { m_sync.ExitWriteLock(); }
        }
        public bool Remove(TEntity item)
        {
            m_sync.EnterWriteLock();
            bool remove = false;

            try
            {
                if (m_storage.Remove(item))
                {
                    Interlocked.Decrement(ref m_count);
                    remove = true;
                }
            }

            finally
            { m_sync.ExitWriteLock(); }

            return remove;
        }

        public void RemoveAt(int index)
        {
            m_sync.EnterWriteLock();

            try
            { m_storage.RemoveAt(index); }

            finally
            { m_sync.ExitWriteLock(); }
        }

        public void ForEach(Action<TEntity> action)
        {
            m_sync.EnterWriteLock();

            try
            { m_storage.ForEach(action); }

            finally
            { m_sync.ExitWriteLock(); }
        }

        public IEnumerable<TEntity> Where(Func<TEntity, bool> predicat)
        {
            m_sync.EnterReadLock();

            try
            { return m_storage.Where(predicat); }

            finally
            { m_sync.ExitReadLock(); }
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            m_sync.EnterReadLock();

            try
            { return m_storage.GetEnumerator(); }

            finally
            { m_sync.ExitReadLock(); }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            m_sync.EnterReadLock();

            try { return m_storage.GetEnumerator(); }

            finally
            { m_sync.ExitReadLock(); }
        }
    }
}
