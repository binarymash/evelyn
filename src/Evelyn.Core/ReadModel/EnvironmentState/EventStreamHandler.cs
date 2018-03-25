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
                case EnvironmentStateAdded esa:
                    return new ProjectionBuilderRequest(esa.Id, esa.EnvironmentKey);
                case EnvironmentStateDeleted esd:
                    return new ProjectionBuilderRequest(esd.Id, esd.EnvironmentKey);
                case ToggleStateAdded tsa:
                    return new ProjectionBuilderRequest(tsa.Id, tsa.EnvironmentKey);
                case ToggleStateChanged tsc:
                    return new ProjectionBuilderRequest(tsc.Id, tsc.EnvironmentKey);
                case ToggleStateDeleted tsd:
                    return new ProjectionBuilderRequest(tsd.Id, tsd.EnvironmentKey);
                default:
                    throw new InvalidOperationException();
            }
        }

        protected override async Task UpdateProjection(ProjectionBuilderRequest request, CancellationToken token)
        {
            var projectionKey = $"{request.ProjectId}-{request.EnvironmentKey}";

            var dto = await ProjectionBuilder.Invoke(request, token);

            if (dto == null)
            {
                await _db.Delete(projectionKey);
            }
            else
            {
                await _db.AddOrUpdate(projectionKey, dto);
            }
        }
    }
}
