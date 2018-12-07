namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
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

        public async Task Handle(long streamVersion, AccountEvents.AccountRegistered @event, CancellationToken stoppingToken)
        {
            var projection = AccountProjectsDto.Create(CreateEventAudit(streamVersion, @event), @event.Id);
            await Projections.Create(AccountProjectsDto.StoreKey(@event.Id), projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, AccountEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddProject(CreateEventAudit(streamVersion, @event), @event.ProjectId, string.Empty);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, AccountEvents.ProjectDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteProject(CreateEventAudit(streamVersion, @event), @event.ProjectId);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.AccountId);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.SetProjectName(CreateEventAudit(streamVersion, @event, projection.Audit.Version), @event.Id, @event.Name);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }
    }
}
