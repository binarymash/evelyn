namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class EventStreamHandler : EventStreamHandler<ProjectionBuilderRequest, EnvironmentStateDto>
    {
        private IDatabase<string, EnvironmentStateDto> _db;

        public EventStreamHandler(
            IProjectionBuilder<ProjectionBuilderRequest, EnvironmentStateDto> projectionBuilder,
            IDatabase<string, EnvironmentStateDto> db,
            IEventStreamFactory eventQueueFactory)
            : base(projectionBuilder, eventQueueFactory)
        {
            _db = db;
        }

        protected override ProjectionBuilderRequest BuildProjectionRequest(IEvent @event)
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

        protected override async Task UpdateProjection(ProjectionBuilderRequest request, CancellationToken token)
        {
            var dto = await ProjectionBuilder.Invoke(request, token);
            await _db.AddOrUpdate($"{request.ProjectId}-{request.EnvironmentKey}", dto);
        }
    }
}
