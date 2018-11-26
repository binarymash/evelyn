namespace Evelyn.Core.ReadModel.EventStream
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class EventStream
    {
        private readonly SemaphoreSlim _sem;
        private readonly ConcurrentQueue<EventEnvelope> _que;

        public EventStream()
        {
            _sem = new SemaphoreSlim(0);
            _que = new ConcurrentQueue<EventEnvelope>();
        }

        public int QueueSize
        {
            get { return _sem.CurrentCount; }
        }

        public void Enqueue(EventEnvelope item)
        {
            _que.Enqueue(item);
            _sem.Release();
        }

        public void EnqueueRange(IEnumerable<EventEnvelope> source)
        {
            var n = 0;
            foreach (var item in source)
            {
                _que.Enqueue(item);
                n++;
            }

            _sem.Release(n);
        }

        public async Task<EventEnvelope> DequeueAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            for (; ;)
            {
                await _sem.WaitAsync(cancellationToken);

                EventEnvelope item;
                if (_que.TryDequeue(out item))
                {
                    return item;
                }
            }
        }
    }
}
