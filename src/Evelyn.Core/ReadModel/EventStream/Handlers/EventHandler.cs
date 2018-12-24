namespace Evelyn.Core.ReadModel.EventStream.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.Projections;
    using Microsoft.Extensions.Logging;

    public class EventHandler<TEventStream> : IEventHandler<TEventStream>
    {
        private readonly ILogger<EventHandler<TEventStream>> _logger;
        private readonly ProjectionBuildersByEventType _projectionBuildersByEventType;
        private readonly IProjectionStore<Projections.EventHandlerState.Projection> _eventHandlerStateStore;

        public EventHandler(ILogger<EventHandler<TEventStream>> logger, IProjectionStore<Projections.EventHandlerState.Projection> eventHandlerStateStore, IProjectionBuilderRegistrar projectionBuilderRegistrar)
        {
            _logger = logger;
            _eventHandlerStateStore = eventHandlerStateStore;
            _projectionBuildersByEventType = projectionBuilderRegistrar.Get(typeof(TEventStream));
        }

        public async Task HandleEvent(EventEnvelope eventEnvelope, CancellationToken stoppingToken)
        {
            try
            {
                bool initialEvent = false;
                var storeKey = Projections.EventHandlerState.Projection.StoreKey(typeof(TEventStream));
                Projections.EventHandlerState.Projection state;

                try
                {
                    state = await _eventHandlerStateStore.Get(storeKey).ConfigureAwait(false);
                    if (eventEnvelope.StreamPosition <= state.Audit.StreamPosition)
                    {
                        _logger.LogInformation("Ignoring event with stream position {@eventStreamPosition} because we have already processed to {@currentStreamPosition}", eventEnvelope.StreamPosition, state.Audit.StreamPosition);
                        return;
                    }
                }
                catch (ProjectionNotFoundException)
                {
                    _logger.LogInformation("No state found");
                    initialEvent = true;
                }

                if (_projectionBuildersByEventType.TryGetValue(eventEnvelope.Event.GetType(), out var projectionBuilders))
                {
                    var tasks = new Task[projectionBuilders.Count];
                    for (var index = 0; index < projectionBuilders.Count; index++)
                    {
                        tasks[index] = projectionBuilders[index](eventEnvelope.StreamPosition, eventEnvelope.Event, stoppingToken);
                    }

                    await Task.WhenAll(tasks);
                }

                var eventAudit = EventAudit.Create(DateTime.UtcNow, Constants.SystemUser, eventEnvelope.Event.Version, eventEnvelope.StreamPosition);

                state = Projections.EventHandlerState.Projection.Create(eventAudit);
                if (initialEvent)
                {
                    await _eventHandlerStateStore.Create(storeKey, state).ConfigureAwait(false);
                }
                else
                {
                    await _eventHandlerStateStore.Update(storeKey, state).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to build projection for {@event}. Exception: {@exception}", eventEnvelope.Event, ex);
                throw;
            }
        }
    }
}
