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
        private IEventStoreConnection _connection;

        public EventStoreImplementation(EventStoreDotOrgOptions options)
        {
            _options = options;
            var connectionSettings = ConnectionSettings.Create()
                .EnableVerboseLogging()
                .UseConsoleLogger()
                .Build();

            //_connection = EventStoreConnection.Create(connectionSettings, "tcp://192.168.1.64:2113");
            _connection = EventStoreConnection.Create(connectionSettings, new Uri("tcp://192.168.1.64:1113"), "MyConnection");

            _connection.AuthenticationFailed += OnAuthenticationFailed;
            _connection.Connected += OnConnected;
            _connection.Disconnected += OnDisconnected;
            _connection.Reconnecting += OnReconnecting;
            _connection.Closed += OnClosed;
            _connection.ErrorOccurred += OnErrorOccurred;

            _connection.ConnectAsync().Wait();
        }

        private void OnErrorOccurred(object sender, ClientErrorEventArgs clientErrorEventArgs)
        {
            //throw new NotImplementedException();
        }

        private void OnReconnecting(object sender, ClientReconnectingEventArgs clientReconnectingEventArgs)
        {
        //    throw new NotImplementedException();
        }

        private void OnClosed(object sender, ClientClosedEventArgs clientClosedEventArgs)
        {
            //throw new NotImplementedException();
        }

        private void OnDisconnected(object sender, ClientConnectionEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnConnected(object sender, ClientConnectionEventArgs clientConnectionEventArgs)
        {
            //throw new NotImplementedException();
        }

        private void OnAuthenticationFailed(object sender, ClientAuthenticationFailedEventArgs clientAuthenticationFailedEventArgs)
        {
            //throw new NotImplementedException();
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
