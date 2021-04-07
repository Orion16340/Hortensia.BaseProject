using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Hortensia.Threads.Sync.Queue.LockFree
{
    public class LockFreeQueue<TEntity> : IEnumerable<TEntity>
    {
        private LinkedNode<TEntity> m_head;
        private LinkedNode<TEntity> m_tail;
        private int m_count;

        public int Count { get => Thread.VolatileRead(ref m_count); }


        public LockFreeQueue()
        {
            this.m_head = new LinkedNode<TEntity>();
            this.m_tail = m_head;
        }


        public void Enqueue(TEntity item)
        {
            LinkedNode<TEntity> oldTail = default;
            var newNode = new LinkedNode<TEntity>() { Item = item };
            bool nodeAdded = false;

            while (!nodeAdded)
            {
                oldTail = m_tail;
                LinkedNode<TEntity> oldTailNext = oldTail.Next;

                if (m_tail != oldTail)
                    continue;
                else if (oldTailNext == null)
                    nodeAdded = Interlocked.CompareExchange(ref m_tail.Next, newNode, null) == null;
                else Interlocked.CompareExchange(ref m_tail, oldTailNext, oldTail);
            }
            Interlocked.CompareExchange(ref m_tail, newNode, oldTail);
            Interlocked.Increment(ref m_count);
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the queue.
        /// </summary>
        /// <param name="item">
        /// when the method returns, contains the object removed from the beginning of the queue, 
        /// if the queue is not empty; otherwise it is the default value for the element type
        /// </param>
        /// <returns>
        /// true if an object from removed from the beginning of the queue; 
        /// false if the queue is empty
        /// </returns>
        public bool TryDequeue(out TEntity item)
        {
            item = default;

            bool haveAdvancedHead = false;
            while (!haveAdvancedHead)
            {
                var oldHead = m_head;
                var oldTail = m_tail;
                var oldHeadNext = oldHead.Next;

                if (oldHead != m_head)
                    continue;

                if (oldHead == oldTail)
                {
                    if (oldHeadNext == null)
                        return false;

                    Interlocked.CompareExchange(ref m_tail, oldHeadNext, oldTail);
                }

                else
                {
                    item = oldHeadNext.Item;
                    haveAdvancedHead = Interlocked.CompareExchange(ref m_head, oldHeadNext, oldHead) == oldHead;
                }
            }

            Interlocked.Decrement(ref m_count);
            return true;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            do
            {
                if (m_head.Item == null) yield break;
                yield return m_head.Item;
            } while ((m_head = m_head.Next) != null);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

    }
}
