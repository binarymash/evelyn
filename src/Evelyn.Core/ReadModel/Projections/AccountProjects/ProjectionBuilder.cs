namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
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
            var projection = AccountProjectsDto.Create(@event.Id, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Create(AccountProjectsDto.StoreKey(@event.Id), projection).ConfigureAwait(false);
        }

        public async Task Handle(AccountEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddProject(@event.ProjectId, string.Empty, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(AccountEvents.ProjectDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteProject(@event.ProjectId, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.AccountId);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.SetProjectName(@event.Id, @event.Name, @event.OccurredAt, @event.UserId, projection.Audit.Version);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }
    }
}
