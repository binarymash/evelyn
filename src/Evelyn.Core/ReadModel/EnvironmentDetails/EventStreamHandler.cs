namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;
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
            switch (@event)
            {
                case EnvironmentAdded ea:
                    return new ProjectionBuilderRequest(ea.Id, ea.Key);
                case EnvironmentDeleted ed:
                    return new ProjectionBuilderRequest(ed.Id, ed.Key);
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
