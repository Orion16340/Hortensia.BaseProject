namespace Hortensia.Threads.Sync.Queue.LockFree
{
    internal class LinkedNode<TEntity>
    {
        public TEntity Item { get; set; }

        public LinkedNode<TEntity> Next; /* Not {get;set;} for thread safety check using of Next in LockFreeQueue.cs */
    }
}
