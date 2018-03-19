namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using CQRSlite.Events;

    public class EventStreamFactory : IEventStreamFactory
    {
        private readonly Dictionary<Type, Queue<IEvent>> _queues;

        public EventStreamFactory()
        {
            _queues = new Dictionary<Type, Queue<IEvent>>();
        }

        public Queue<IEvent> GetEventStream<T>()
        {
            if (!_queues.TryGetValue(typeof(T), out var queue))
            {
                queue = new Queue<IEvent>();
                _queues.Add(typeof(T), queue);
            }

            return queue;
        }
    }
}
