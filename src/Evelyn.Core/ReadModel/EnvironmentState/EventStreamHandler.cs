namespace Evelyn.Core.ReadModel.EnvironmentState
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
        private readonly IProjectionBuilder<ProjectionBuilderRequest, EnvironmentStateDto> _projectionBuilder;
        private readonly IDatabase<string, EnvironmentStateDto> _db;
        private readonly Queue<IEvent> _eventsToHandle;

        public EventStreamHandler(IProjectionBuilder<ProjectionBuilderRequest, EnvironmentStateDto> projectionBuilder, IDatabase<string, EnvironmentStateDto> db, IEventStreamFactory eventQueueFactory)
        {
            _projectionBuilder = projectionBuilder;
            _db = db;
            _eventsToHandle = eventQueueFactory.GetEventStream<EnvironmentStateDto>();
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
                case EnvironmentAdded ea:
                    return new ProjectionBuilderRequest(ea.Id, ea.Key);
                case ToggleAdded ta:
                    return new ProjectionBuilderRequest(ta.Id, ta.Key);
                case ToggleStateChanged tsc:
                    return new ProjectionBuilderRequest(tsc.Id, tsc.EnvironmentKey);
                default:
                    throw new InvalidOperationException();
            }
        }

        private async Task UpdateProjection(ProjectionBuilderRequest request, CancellationToken token)
        {
            var dto = await _projectionBuilder.Invoke(request, token);
            await _db.AddOrUpdate($"{request.ProjectId}-{request.EnvironmentKey}", dto);
        }
    }
}
