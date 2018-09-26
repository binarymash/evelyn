namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.EventStreamHandlerState;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class EventStreamHandler : BackgroundService
    {
        private const string StoreKey = nameof(EventStreamHandler);
        private readonly ILogger<EventStreamHandler> _logger;
        private readonly Queue<EventEnvelope> _eventsToHandle;
        private readonly Dictionary<Type, List<Func<IEvent, CancellationToken, Task>>> _projectionBuilders;
        private readonly IProjectionStore<EventStreamHandlerStateDto> _projections;
        private readonly IProjectionBuilderRegistrar _projectionBuilderRegistrar;

        public EventStreamHandler(ILogger<EventStreamHandler> logger, IEventStreamFactory eventStreamFactory, IProjectionStore<EventStreamHandlerStateDto> projectionStore, IProjectionBuilderRegistrar projectionBuilderRegistrar)
        {
            _logger = logger;
            _eventsToHandle = eventStreamFactory.GetEventStream<EventStreamHandler>();
            _projectionBuilders = new Dictionary<Type, List<Func<IEvent, CancellationToken, Task>>>();
            _projections = projectionStore;
            _projectionBuilderRegistrar = projectionBuilderRegistrar;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var projectionBuilders = _projectionBuilderRegistrar.Get(typeof(EventStreamHandler));
            foreach (var @event in projectionBuilders.Keys)
            {
                _projectionBuilders.Add(@event, projectionBuilders[@event]);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_eventsToHandle.Count > 0)
                {
                    var eventEnvelope = _eventsToHandle.Peek();
                    await HandleEvent(eventEnvelope, stoppingToken).ConfigureAwait(false);
                    _eventsToHandle.Dequeue();
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                }
            }
        }

        protected async Task HandleEvent(EventEnvelope eventEnvelope, CancellationToken stoppingToken)
        {
            try
            {
                bool initialEvent = false;
                EventStreamHandlerStateDto state;

                try
                {
                    state = await _projections.Get(StoreKey).ConfigureAwait(false);
                }
                catch (ProjectionNotFoundException)
                {
                    _logger.LogInformation("No state found");
                    state = EventStreamHandlerStateDto.Null;
                    initialEvent = true;
                }

                if (state.Version < eventEnvelope.StreamVersion)
                {
                    if (_projectionBuilders.TryGetValue(eventEnvelope.Event.GetType(), out var handlers))
                    {
                        var tasks = new Task[handlers.Count];
                        for (var index = 0; index < handlers.Count; index++)
                        {
                            tasks[index] = handlers[index](eventEnvelope.Event, stoppingToken);
                        }

                        await Task.WhenAll(tasks);
                    }

                    state.Processed(eventEnvelope.StreamVersion, DateTime.UtcNow, Constants.SystemUser);
                    if (initialEvent)
                    {
                        await _projections.Create(StoreKey, state).ConfigureAwait(false);
                    }
                    else
                    {
                        await _projections.Update(StoreKey, state).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to build projection for {@event}. Exception: {@exception}", eventEnvelope.Event, ex);
            }
        }
    }
}
