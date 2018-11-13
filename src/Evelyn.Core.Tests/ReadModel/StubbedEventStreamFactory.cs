namespace Evelyn.Core.Tests.ReadModel
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Core.ReadModel;

    public class StubbedEventStreamFactory : IEventStreamFactory
    {
        private readonly EventStream _eventStream;

        public StubbedEventStreamFactory()
        {
            _eventStream = new EventStream();
        }

        public EventStream GetEventStream<TDto>()
        {
            return _eventStream;
        }
    }
}
