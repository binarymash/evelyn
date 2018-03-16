namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class EventStreamHandler : BackgroundService
    {
        private readonly IProjectionBuilder<ProjectionBuilderRequest, ToggleDetailsDto> _projectionBuilder;
        private readonly IDatabase<string, ToggleDetailsDto> _db;
        private readonly Queue<IEvent> _eventsToHandle;

        public EventStreamHandler(IProjectionBuilder<ProjectionBuilderRequest, ToggleDetailsDto> projectionBuilder, IDatabase<string, ToggleDetailsDto> db, IEventStreamFactory eventQueueFactory)
        {
            _projectionBuilder = projectionBuilder;
            _db = db;
            _eventsToHandle = eventQueueFactory.GetEventStream<ToggleDetailsDto>();
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
            switch (@event)
            {
                case ToggleAdded ta:
                    return new ProjectionBuilderRequest(ta.Id, ta.Key);
                default:
                    throw new InvalidOperationException();
            }
        }

        private async Task UpdateProjection(ProjectionBuilderRequest request, CancellationToken token)
        {
            var dto = await _projectionBuilder.Invoke(request, token);
            await _db.AddOrUpdate($"{request.ProjectId}-{request.ToggleKey}", dto);
        }
    }
}
