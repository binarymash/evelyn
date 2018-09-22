namespace Evelyn.Core.Tests.ReadModel
{
    using System.Collections.Generic;
    using Core.ReadModel;
    using CQRSlite.Events;

    public class StubbedEventStreamFactory : IEventStreamFactory
    {
        private readonly Queue<EventEnvelope> _events;

        public StubbedEventStreamFactory()
        {
            _events = new Queue<EventEnvelope>();
        }

        public Queue<EventEnvelope> GetEventStream<TDto>()
        {
            return _events;
        }
    }
}
