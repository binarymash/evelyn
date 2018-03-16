namespace Evelyn.Core.ReadModel.EnvironmentDetails
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
        : ICancellableEventHandler<EnvironmentAdded>
    {
        private readonly IProjectionBuilder<ProjectionBuilderRequest, EnvironmentDetailsDto> _projectionBuilder;
        private readonly IDatabase<string, EnvironmentDetailsDto> _db;

        public Handler(IProjectionBuilder<ProjectionBuilderRequest, EnvironmentDetailsDto> projectionBuilder, IDatabase<string, EnvironmentDetailsDto> db)
        {
            _projectionBuilder = projectionBuilder;
            _db = db;
        }

        public async Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            await UpdateProjection(message.Id, message.Key, token);
        }

        public async Task UpdateProjection(Guid projectId, string environmentKey, CancellationToken token)
        {
            var dto = await _projectionBuilder.Invoke(new ProjectionBuilderRequest(projectId, environmentKey), token);
            await _db.AddOrUpdate($"{projectId}-{environmentKey}", dto);
        }
    }
}
