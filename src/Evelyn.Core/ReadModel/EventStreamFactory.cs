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

        public Queue<EventEnvelope> GetEventStream<T>()
        {
            if (!_queues.TryGetValue(typeof(T), out var queue))
            {
                queue = new Queue<EventEnvelope>();
                _queues.Add(typeof(T), queue);
            }

            return queue;
        }
    }
}
