namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;

    public class EventStreamHandler : EventStreamHandler<ProjectionBuilderRequest, AccountProjectsDto>
    {
        private readonly IDatabase<Guid, AccountProjectsDto> _db;

        public EventStreamHandler(
            IProjectionBuilder<ProjectionBuilderRequest, AccountProjectsDto> projectionBuilder,
            IDatabase<Guid, AccountProjectsDto> db,
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
            await _db.AddOrUpdate(dto.AccountId, dto);
        }
    }
}
