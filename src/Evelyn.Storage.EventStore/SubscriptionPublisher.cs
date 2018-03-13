namespace Evelyn.Storage.EventStore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using global::EventStore.ClientAPI;

    public class SubscriptionPublisher : BackgroundService
    {
        private const string EvelynEvents = "$ce-evelyn";
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventStoreConnection _connection;
        private readonly EventMapper _eventMapper;

        public SubscriptionPublisher(IEventStoreConnectionFactory connectionFactory, IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
            _eventMapper = new EventMapper();
            _connection = connectionFactory.Invoke();
            _connection.ConnectAsync().Wait();
        }

        private Action<EventStoreCatchUpSubscription> OnLiveProcessingStarted => (s) =>
        {
        };

        private Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> OnSubscriptionDropped => (s, r, e) =>
        {
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
            // TODO: what should we do if this fails?
            var evelynEvent = _eventMapper.MapEvent(@event);
            return _eventPublisher.Publish(evelynEvent);
        }
    }
}
