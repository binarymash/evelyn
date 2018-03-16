namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class EventStreamHandler : EventStreamHandler<ProjectionBuilderRequest, ToggleDetailsDto>
    {
        private IDatabase<string, ToggleDetailsDto> _db;

        public EventStreamHandler(
            IProjectionBuilder<ProjectionBuilderRequest, ToggleDetailsDto> projectionBuilder,
            IDatabase<string, ToggleDetailsDto> db,
            IEventStreamFactory eventQueueFactory)
            : base(projectionBuilder, eventQueueFactory)
        {
            _db = db;
        }

        protected override ProjectionBuilderRequest BuildProjectionRequest(IEvent @event)
        {
            switch (@event)
            {
                case ToggleAdded ta:
                    return new ProjectionBuilderRequest(ta.Id, ta.Key);
                default:
                    throw new InvalidOperationException();
            }
        }

        protected override async Task UpdateProjection(ProjectionBuilderRequest request, CancellationToken token)
        {
            var dto = await ProjectionBuilder.Invoke(request, token);
            await _db.AddOrUpdate($"{request.ProjectId}-{request.ToggleKey}", dto);
        }
    }
}
