namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;

    public class EventStreamFactory : IEventStreamFactory
    {
        private readonly Dictionary<Type, Queue<EventEnvelope>> _queues;

        public EventStreamFactory()
        {
            _queues = new Dictionary<Type, Queue<EventEnvelope>>();
        }

        public Queue<EventEnvelope> GetEventStream<TEventStreamHandler>()
        {
            if (!_queues.TryGetValue(typeof(TEventStreamHandler), out var queue))
            {
                queue = new Queue<EventEnvelope>();
                _queues.Add(typeof(TEventStreamHandler), queue);
            }

            return queue;
        }
    }
}
