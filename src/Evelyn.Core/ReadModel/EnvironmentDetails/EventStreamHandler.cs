namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class EventStreamHandler : EventStreamHandler<ProjectionBuilderRequest, EnvironmentDetailsDto>
    {
        private readonly IDatabase<string, EnvironmentDetailsDto> _db;

        public EventStreamHandler(
            IProjectionBuilder<ProjectionBuilderRequest, EnvironmentDetailsDto> projectionBuilder,
            IDatabase<string, EnvironmentDetailsDto> db,
            IEventStreamFactory eventQueueFactory)
            : base(projectionBuilder, eventQueueFactory)
        {
            _db = db;
        }

        protected override ProjectionBuilderRequest BuildProjectionRequest(IEvent @event)
        {
            var environmentAddedEvent = @event as EnvironmentAdded;
            return new ProjectionBuilderRequest(environmentAddedEvent.Id, environmentAddedEvent.Key);
        }

        protected override async Task UpdateProjection(ProjectionBuilderRequest request, CancellationToken token)
        {
            var dto = await ProjectionBuilder.Invoke(request, token);
            await _db.AddOrUpdate($"{dto.ProjectId}-{dto.Key}", dto);
        }
    }
}
