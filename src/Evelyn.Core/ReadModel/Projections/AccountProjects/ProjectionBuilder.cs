namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AccountEvents = Evelyn.Core.WriteModel.Account.Events;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<AccountProjectsDto>,
        IBuildProjectionsFrom<AccountEvents.AccountRegistered>,
        IBuildProjectionsFrom<AccountEvents.ProjectCreated>,
        IBuildProjectionsFrom<AccountEvents.ProjectDeleted>,
        IBuildProjectionsFrom<ProjectEvents.ProjectCreated>
    {
        public ProjectionBuilder(IProjectionStore<AccountProjectsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(AccountEvents.AccountRegistered @event, CancellationToken stoppingToken)
        {
            var projection = AccountProjectsDto.Create(@event.Id, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(AccountProjectsDto.StoreKey(@event.Id), projection).ConfigureAwait(false);
        }

        public async Task Handle(AccountEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddProject(@event.ProjectId, string.Empty, @event.Version, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(AccountEvents.ProjectDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteProject(@event.ProjectId, @event.Version, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.AccountId);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            var project = projection.Projects.Single(p => p.Id == @event.Id);
            project.SetName(@event.Name);
            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }
    }
}
