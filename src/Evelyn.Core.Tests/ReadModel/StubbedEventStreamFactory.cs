namespace Evelyn.Core.Tests.ReadModel
{
    using System.Collections.Generic;
    using Core.ReadModel;
    using CQRSlite.Events;

    public class StubbedEventStreamFactory : IEventStreamFactory
    {
        private readonly Queue<IEvent> _events;

        public StubbedEventStreamFactory()
        {
            _events = new Queue<IEvent>();
        }

        public Queue<IEvent> GetEventStream<TDto>()
        {
            return _events;
        }
    }
}
