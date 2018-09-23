namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Evelyn.Core.WriteModel.Account.Events;

    public class ProjectionBuilder : ProjectionBuilder<AccountProjectsDto>,
        IBuildProjectionsFrom<AccountRegistered>,
        IBuildProjectionsFrom<ProjectCreated>,
        IBuildProjectionsFrom<WriteModel.Project.Events.ProjectCreated>,
        IBuildProjectionsFrom<ProjectDeleted>
    {
        public ProjectionBuilder(IProjectionStore<AccountProjectsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(AccountRegistered @event, CancellationToken stoppingToken)
        {
            var projection = new AccountProjectsDto(@event.Id, @event.Version, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId, new List<ProjectListDto>());
            await Projections.AddOrUpdate(AccountProjectsDto.StoreKey(@event.Id), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectCreated @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddProject(@event.ProjectId, string.Empty, @event.Version, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(WriteModel.Project.Events.ProjectCreated @event, CancellationToken stoppingToken)
        {
                var storeKey = AccountProjectsDto.StoreKey(@event.AccountId);
                var projection = await Projections.Get(storeKey).ConfigureAwait(false);
                var project = projection.Projects.Single(p => p.Id == @event.Id);
                project.SetName(@event.Name);
                await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteProject(@event.ProjectId, @event.Version, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }
    }
}
