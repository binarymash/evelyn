namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Domain;
    using WriteModel.Project.Events;

    public class Handler
        : ICancellableEventHandler<ToggleAdded>
    {
        private readonly IProjectionBuilder<ProjectionBuilderRequest, ToggleDetailsDto> _projectionBuilder;
        private readonly IDatabase<string, ToggleDetailsDto> _db;

        public Handler(IProjectionBuilder<ProjectionBuilderRequest, ToggleDetailsDto> projectionBuilder, IDatabase<string, ToggleDetailsDto> db)
        {
            _projectionBuilder = projectionBuilder;
            _db = db;
        }

        public async Task Handle(ToggleAdded message, CancellationToken token)
        {
            await UpdateProjection(message.Id, message.Key, token);
        }

        private async Task UpdateProjection(Guid projectId, string toggleKey, CancellationToken token)
        {
            var dto = await _projectionBuilder.Invoke(new ProjectionBuilderRequest(projectId, toggleKey), token);
            await _db.AddOrUpdate($"{projectId}-{toggleKey}", dto);
        }
    }
}
