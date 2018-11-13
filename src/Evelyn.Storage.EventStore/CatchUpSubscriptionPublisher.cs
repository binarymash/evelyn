namespace Evelyn.Storage.EventStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel;
    using global::EventStore.ClientAPI;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class CatchUpSubscriptionPublisher : BackgroundService
    {
        private const string EvelynEvents = "$ce-evelyn";
        private readonly ILogger<CatchUpSubscriptionPublisher> _logger;
        private readonly IEventStoreConnection _connection;
        private readonly EventMapper _eventMapper;
        private readonly EventStream _eventStream;

        public CatchUpSubscriptionPublisher(ILogger<CatchUpSubscriptionPublisher> logger, IEventStoreConnectionFactory connectionFactory, IEventStreamFactory eventStreamFactory)
        {
            _logger = logger;
            _eventMapper = new EventMapper();
            _eventStream = eventStreamFactory.GetEventStream<EventStreamHandler>();
            _connection = connectionFactory.Invoke();
            _connection.ConnectAsync().Wait();
        }

        private Action<EventStoreCatchUpSubscription> OnLiveProcessingStarted => (s) =>
        {
        };

        private Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> OnSubscriptionDropped => (s, r, e) =>
        {
            _logger.LogInformation("Subscription dropped. Subscription: {@subscription}; Reason:{@reason}; Exception: {@exception}", s.StreamId, r, e);
        };

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var settings = new CatchUpSubscriptionSettings(100, 100, false, true);
            var subscription = _connection.SubscribeToStreamFrom(EvelynEvents, null, settings, OnEventAppeared, OnLiveProcessingStarted, OnSubscriptionDropped);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            subscription.Stop();
        }

        private Task OnEventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent @event)
        {
            _logger.LogInformation("Received event on subscription {@subscriptionName}: {@event}", subscription.SubscriptionName, @event);

            // TODO: what should we do if this fails?
            var evelynEvent = _eventMapper.MapEvent(@event);
            var eventEnvelope = new EventEnvelope(@event.OriginalEventNumber, evelynEvent);
            _eventStream.Enqueue(eventEnvelope);
            return Task.CompletedTask;
        }
    }
}
