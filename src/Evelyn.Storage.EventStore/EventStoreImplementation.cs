namespace Evelyn.Storage.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using global::EventStore.ClientAPI;

    public class EventStoreImplementation : IEventStore
    {
        private const int SliceSize = 200;
        private const bool ResolveLinkTos = false;

        private readonly IEventStoreConnection _connection;
        private readonly EventMapper _eventMapper;

        public EventStoreImplementation(IEventStoreConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.Invoke();
            _connection.ConnectAsync().Wait();
            _eventMapper = new EventMapper();
        }

#pragma warning disable SA1129 // Do not use default value type constructor
        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
#pragma warning restore SA1129 // Do not use default value type constructor
        {
            var expectedVersion = MapExpectedVersion(events);
            var streamName = MapStreamName(events);
            var eventStoreEvents = events.Select(_eventMapper.MapEvent).ToArray();

            await _connection.AppendToStreamAsync(streamName, expectedVersion, eventStoreEvents);
        }

#pragma warning disable SA1129 // Do not use default value type constructor
        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken())
#pragma warning restore SA1129 // Do not use default value type constructor
        {
            var streamEvents = new List<ResolvedEvent>();

            StreamEventsSlice currentSlice;
            var startPosition = (long)fromVersion + 1;

            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(
                    MapStreamName(aggregateId),
                    startPosition,
                    SliceSize,
                    ResolveLinkTos);

                startPosition = currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);
            }
            while (!currentSlice.IsEndOfStream);

            var mappedEvents = streamEvents.Select(_eventMapper.MapEvent);
            var eventsToReturn = mappedEvents.Where(e => e.Version > fromVersion);
            return eventsToReturn;
        }

        private long MapExpectedVersion(IEnumerable<IEvent> events)
        {
            return events.First().Version - 1;
        }

        private string MapStreamName(IEnumerable<IEvent> events)
        {
            return $"project-{events.First().Id}";
        }

        private string MapStreamName(Guid projectId)
        {
            return $"project-{projectId}";
        }
    }
}
