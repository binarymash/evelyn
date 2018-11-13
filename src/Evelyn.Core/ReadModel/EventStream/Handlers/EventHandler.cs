namespace Evelyn.Core.ReadModel.EventStream.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.EventHandlerState;
    using Microsoft.Extensions.Logging;

    public class EventHandler<TEventStream> : IEventHandler<TEventStream>
    {
        private readonly ILogger<EventHandler<TEventStream>> _logger;
        private readonly ProjectionBuildersByEventType _projectionBuildersByEventType;
        private readonly IProjectionStore<EventHandlerStateDto> _eventHandlerStateStore;

        public EventHandler(ILogger<EventHandler<TEventStream>> logger, IProjectionStore<EventHandlerStateDto> eventHandlerStateStore, IProjectionBuilderRegistrar projectionBuilderRegistrar)
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
                EventHandlerStateDto state;

                state = await _eventHandlerStateStore.Get(EventHandlerStateDto.StoreKey(typeof(TEventStream))).ConfigureAwait(false);

                if (state.Version == EventHandlerStateDto.Null.Version)
                {
                    _logger.LogInformation("No state found");
                    initialEvent = true;
                }

                if (state.Version < eventEnvelope.StreamVersion)
                {
                    if (_projectionBuildersByEventType.TryGetValue(eventEnvelope.Event.GetType(), out var projectionBuilders))
                    {
                        var tasks = new Task[projectionBuilders.Count];
                        for (var index = 0; index < projectionBuilders.Count; index++)
                        {
                            tasks[index] = projectionBuilders[index](eventEnvelope.Event, stoppingToken);
                        }

                        await Task.WhenAll(tasks);
                    }

                    state.Processed(eventEnvelope.StreamVersion, DateTime.UtcNow, Constants.SystemUser);
                    if (initialEvent)
                    {
                        await _eventHandlerStateStore.Create(EventHandlerStateDto.StoreKey(typeof(TEventStream)), state).ConfigureAwait(false);
                    }
                    else
                    {
                        await _eventHandlerStateStore.Update(EventHandlerStateDto.StoreKey(typeof(TEventStream)), state).ConfigureAwait(false);
                    }
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
