namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.EventStreamHandlerState;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class EventStreamHandler : BackgroundService
    {
        private const string StoreKey = nameof(EventStreamHandler);
        private readonly ILogger<EventStreamHandler> _logger;
        private readonly Queue<EventEnvelope> _eventsToHandle;
        private readonly List<IProjectionBuilder> _projectionBuilders;
        private readonly IProjectionStore<EventStreamHandlerStateDto> _projections;
        private readonly IProjectionBuilderRegistrar _projectionBuilderRegistrar;

        public EventStreamHandler(ILogger<EventStreamHandler> logger, IEventStreamFactory eventStreamFactory, IProjectionStore<EventStreamHandlerStateDto> projectionStore, IProjectionBuilderRegistrar projectionBuilderRegistrar)
        {
            _logger = logger;
            _eventsToHandle = eventStreamFactory.GetEventStream<EventStreamHandler>();
            _projectionBuilders = new List<IProjectionBuilder>();
            _projections = projectionStore;
            _projectionBuilderRegistrar = projectionBuilderRegistrar;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _projectionBuilders.AddRange(_projectionBuilderRegistrar.Get(typeof(EventStreamHandler)));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_eventsToHandle.Count > 0)
                {
                    var eventEnvelope = _eventsToHandle.Peek();
                    await HandleEvent(eventEnvelope).ConfigureAwait(false);
                    _eventsToHandle.Dequeue();
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                }
            }
        }

        protected async Task HandleEvent(EventEnvelope eventEnvelope)
        {
            EventStreamHandlerStateDto state;
            try
            {
                state = await _projections.Get(StoreKey).ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                _logger.LogInformation("No state found");
                state = EventStreamHandlerStateDto.Null;
            }

            if (state.Version < eventEnvelope.StreamVersion)
            {
                try
                {
                    foreach (var projectionBuilder in _projectionBuilders)
                    {
                        await projectionBuilder.HandleEvent(eventEnvelope.Event).ConfigureAwait(false);
                    }

                    state.Processed(eventEnvelope.StreamVersion, DateTime.UtcNow, Constants.SystemUser);
                    await _projections.AddOrUpdate(StoreKey, state).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to build projection for {@event}. Exception: {@exception}", eventEnvelope.Event, ex);
                }
            }
        }
    }
}
