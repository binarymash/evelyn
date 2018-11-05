namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class EventStreamHandler : BackgroundService
    {
        private readonly ILogger<EventStreamHandler> _logger;
        private readonly Queue<EventEnvelope> _eventsToHandle;
        private readonly IEventHandler<EventStreamHandler> _eventHandler;

        public EventStreamHandler(ILogger<EventStreamHandler> logger, IEventStreamFactory eventStreamFactory, IEventHandler<EventStreamHandler> eventHandler)
        {
            _logger = logger;
            _eventsToHandle = eventStreamFactory.GetEventStream<EventStreamHandler>();
            _eventHandler = eventHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_eventsToHandle.Count > 0)
                {
                    var eventEnvelope = _eventsToHandle.Peek();
                    await _eventHandler.HandleEvent(eventEnvelope, stoppingToken).ConfigureAwait(false);
                    _eventsToHandle.Dequeue();
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                }
            }
        }
    }
}
