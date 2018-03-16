namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;

    public abstract class EventStreamHandler<TProjectionBuilderRequest, TDto> : BackgroundService
    {
        private readonly Queue<IEvent> _eventsToHandle;

        protected EventStreamHandler(IProjectionBuilder<TProjectionBuilderRequest, TDto> projectionBuilder, IEventStreamFactory eventQueueFactory)
        {
            ProjectionBuilder = projectionBuilder;
            _eventsToHandle = eventQueueFactory.GetEventStream<TDto>();
        }

        protected IProjectionBuilder<TProjectionBuilderRequest, TDto> ProjectionBuilder { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_eventsToHandle.Count > 0)
                {
                    var @event = _eventsToHandle.Dequeue();
                    var request = BuildProjectionRequest(@event);
                    await UpdateProjection(request, stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                }
            }
        }

        protected abstract TProjectionBuilderRequest BuildProjectionRequest(IEvent @event);

        protected abstract Task UpdateProjection(TProjectionBuilderRequest request, CancellationToken token);
    }
}
