namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Microsoft.Extensions.Hosting;

    public abstract class EventStreamHandler<TDto> : BackgroundService
    {
        private readonly Queue<IEvent> _eventsToHandle;

        protected EventStreamHandler(IEventStreamFactory eventQueueFactory)
        {
            _eventsToHandle = eventQueueFactory.GetEventStream<TDto>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_eventsToHandle.Count > 0)
                {
                    var @event = _eventsToHandle.Peek();
                    await HandleEvent(@event).ConfigureAwait(false);
                    _eventsToHandle.Dequeue();
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                }
            }
        }

        protected abstract Task HandleEvent(IEvent @event);
    }
}
