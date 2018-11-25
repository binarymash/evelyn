namespace Evelyn.Core.ReadModel.EventStream
{
    using System;
    using System.Collections.Generic;

    public class EventStreamFactory : IEventStreamFactory
    {
        private readonly Dictionary<Type, EventStream> _eventStreams;

        public EventStreamFactory()
        {
            _eventStreams = new Dictionary<Type, EventStream>();
        }

        public EventStream GetEventStream<TEventStreamHandler>()
        {
            if (!_eventStreams.TryGetValue(typeof(TEventStreamHandler), out var eventStream))
            {
                eventStream = new EventStream();
                _eventStreams.Add(typeof(TEventStreamHandler), eventStream);
            }

            return eventStream;
        }
    }
}
