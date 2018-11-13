namespace Evelyn.Core.ReadModel.EventSubscription
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Evelyn.Core;
    using Evelyn.Core.ReadModel;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class InMemoryEventStoreCatchUpSubscriber : BackgroundService
    {
        private readonly ILogger<InMemoryEventStoreCatchUpSubscriber> _logger;
        private readonly IInMemoryEventStore _eventStore;
        private readonly EventStream _eventStream;

        public InMemoryEventStoreCatchUpSubscriber(ILogger<InMemoryEventStoreCatchUpSubscriber> logger, IEventStreamFactory eventStreamFactory, IInMemoryEventStore eventStore)
        {
            _logger = logger;
            _eventStore = eventStore;
            _eventStream = eventStreamFactory.GetEventStream<EventStreamHandler>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastPosition = -1L;
            while (!stoppingToken.IsCancellationRequested)
            {
                var stream = _eventStore.Get(lastPosition);

                foreach (var eventEnvelope in stream)
                {
                    await OnEventAppeared(eventEnvelope);
                    lastPosition = eventEnvelope.Position;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
            }
        }

        private Task OnEventAppeared(InMemoryEventEnvelope @event)
        {
            var eventEnvelope = MapEvent(@event);
            _eventStream.Enqueue(eventEnvelope);
            return Task.CompletedTask;
        }

        private EventEnvelope MapEvent(InMemoryEventEnvelope eventEnvelope)
        {
            return new EventEnvelope(eventEnvelope.Position, eventEnvelope.Event);
        }
    }
}
