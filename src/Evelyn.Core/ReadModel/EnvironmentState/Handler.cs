namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class Handler :
          ICancellableEventHandler<EnvironmentAdded>,
          ICancellableEventHandler<ToggleAdded>,
          ICancellableEventHandler<ToggleStateChanged>
    {
        private readonly IProjectionBuilder<ProjectionBuilderRequest, EnvironmentStateDto> _projectionBuilder;
        private readonly IDatabase<string, EnvironmentStateDto> _db;

        public Handler(IProjectionBuilder<ProjectionBuilderRequest, EnvironmentStateDto> projectionBuilder, IDatabase<string, EnvironmentStateDto> db)
        {
            _projectionBuilder = projectionBuilder;
            _db = db;
        }

        public static string GetEnvironmentStateKey(Guid projectId, string environmentName)
        {
            return $"{projectId}-{environmentName}";
        }

        public async Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            await UpdateProjection(message.Id, message.Key, token);
        }

        public async Task Handle(ToggleAdded message, CancellationToken token)
        {
            await UpdateProjection(message.Id, message.Key, token);
        }

        public async Task Handle(ToggleStateChanged message, CancellationToken token)
        {
            await UpdateProjection(message.Id, message.EnvironmentKey, token);
        }

        private async Task UpdateProjection(Guid projectId, string environmentKey, CancellationToken token)
        {
            var dto = await _projectionBuilder.Invoke(new ProjectionBuilderRequest(projectId, environmentKey), token);
            await _db.AddOrUpdate($"{projectId}-{environmentKey}", dto);
        }
    }
}
