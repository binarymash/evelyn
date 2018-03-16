namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;

    public class EventStreamHandler : BackgroundService
    {
        private readonly IProjectionBuilder<ProjectionBuilderRequest, ProjectDetailsDto>
            _projectionBuilder;

        private readonly IDatabase<Guid, ProjectDetailsDto> _db;
        private readonly Queue<IEvent> _eventsToHandle;

        public EventStreamHandler(
            IProjectionBuilder<ProjectionBuilderRequest, ProjectDetailsDto> projectionBuilder,
            IDatabase<Guid, ProjectDetailsDto> db,
            IEventStreamFactory eventQueueFactory)
        {
            _projectionBuilder = projectionBuilder;
            _db = db;
            _eventsToHandle = eventQueueFactory.GetEventStream<ProjectDetailsDto>();
        }

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

        private ProjectionBuilderRequest BuildProjectionRequest(IEvent @event)
        {
            return new ProjectionBuilderRequest(@event.Id);
        }

        private async Task UpdateProjection(ProjectionBuilderRequest request, CancellationToken token)
        {
            var dto = await _projectionBuilder.Invoke(request, token).ConfigureAwait(false);
            await _db.AddOrUpdate(request.ProjectId, dto).ConfigureAwait(false);
        }
    }
}
