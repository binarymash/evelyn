namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Infrastructure;
    using WriteModel.Project.Events;

    public class Handler :
        ICancellableEventHandler<ProjectCreated>,
        ICancellableEventHandler<EnvironmentAdded>,
        ICancellableEventHandler<ToggleAdded>
    {
        private readonly IProjectionBuilder<ProjectionBuilderRequest, ProjectDetailsDto> _projectionBuilder;
        private readonly IDatabase<Guid, ProjectDetailsDto> _db;

        public Handler(IProjectionBuilder<ProjectionBuilderRequest, ProjectDetailsDto> projectionBuilder, IDatabase<Guid, ProjectDetailsDto> db)
        {
            _projectionBuilder = projectionBuilder;
            _db = db;
        }

        public async Task Handle(ProjectCreated message, CancellationToken token)
        {
            await UpdateProjection(message.Id, token);
        }

        public async Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            await UpdateProjection(message.Id, token);
        }

        public async Task Handle(ToggleAdded message, CancellationToken token)
        {
            await UpdateProjection(message.Id, token);
        }

        private async Task UpdateProjection(Guid projectId, CancellationToken token)
        {
            var dto = await _projectionBuilder.Invoke(new ProjectionBuilderRequest(projectId), token);
            await _db.AddOrUpdate(projectId, dto);
        }
    }
}
