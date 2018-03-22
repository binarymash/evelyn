namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;

    public class EventStreamHandler : EventStreamHandler<ProjectionBuilderRequest, ProjectDetailsDto>
    {
        private IDatabase<Guid, ProjectDetailsDto> _db;

        public EventStreamHandler(
            IProjectionBuilder<ProjectionBuilderRequest, ProjectDetailsDto> projectionBuilder,
            IDatabase<Guid, ProjectDetailsDto> db,
            IEventStreamFactory eventQueueFactory)
            : base(projectionBuilder, eventQueueFactory)
        {
            _db = db;
        }

        protected override ProjectionBuilderRequest BuildProjectionRequest(IEvent @event)
        {
            return new ProjectionBuilderRequest(@event.Id);
        }

        protected override async Task UpdateProjection(ProjectionBuilderRequest request, CancellationToken token)
        {
            var dto = await ProjectionBuilder.Invoke(request, token);
            await _db.AddOrUpdate(request.ProjectId, dto);
        }
    }
}
