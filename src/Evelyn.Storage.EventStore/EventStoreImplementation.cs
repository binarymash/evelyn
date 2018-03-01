namespace Evelyn.Storage.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using global::EventStore.ClientAPI;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class EventStoreImplementation : IEventStore
    {
        private readonly EventStoreDotOrgOptions _options;

        private readonly IEventStoreConnection _connection;

        private IEventStoreConnectionFactory _connectionFactory;

        public EventStoreImplementation(IEventStoreConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.Invoke();
            _connection.ConnectAsync().Wait();
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            var expectedVersion = events.First().Version - 1;
            var streamName = MapStreamName(events);
            var eventStoreEvents = MapEvents(events);

            await _connection.AppendToStreamAsync(streamName, expectedVersion, eventStoreEvents);
        }

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken())
        {
            var streamEvents = new List<ResolvedEvent>();

            StreamEventsSlice currentSlice;
            var nextSliceStart = (long)StreamPosition.Start;
            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(
                    MapStreamName(aggregateId),
                    nextSliceStart,
                    200,
                    false);

                nextSliceStart = currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);

            var mappedEvents = streamEvents.Select(MapEvent);
            var eventsToReturn = mappedEvents.Where(e => e.Version > fromVersion);
            return eventsToReturn;
        }

        private string MapStreamName(IEnumerable<IEvent> events)
        {
            var @event = events.First();
            return $"application-{@event.Id}";
        }

        private string MapStreamName(Guid applicationId)
        {
            return $"application-{applicationId}";
        }

        private EventData[] MapEvents(IEnumerable<IEvent> events)
        {
            return events.Select(MapEvent).ToArray();
        }

        private EventData MapEvent(IEvent @event)
        {
            return new EventData(
                Guid.NewGuid(),
                @event.GetType().AssemblyQualifiedName,
                false,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                null);
        }

        private IEvent MapEvent(ResolvedEvent @event)
        {
            var type = Type.GetType(@event.Event.EventType);
            var json = Encoding.UTF8.GetString(@event.Event.Data);
            return JsonConvert.DeserializeObject(json, type) as IEvent;
        }
    }
}
