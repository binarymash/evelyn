namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System.Threading;
    using System.Threading.Tasks;
    using AccountEvents = Evelyn.Core.WriteModel.Account.Events;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<Projection>,
        IBuildProjectionsFrom<AccountEvents.AccountRegistered>,
        IBuildProjectionsFrom<AccountEvents.ProjectCreated>,
        IBuildProjectionsFrom<AccountEvents.ProjectDeleted>,
        IBuildProjectionsFrom<ProjectEvents.ProjectCreated>
    {
        public ProjectionBuilder(IProjectionStore<Projection> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(long streamPosition, AccountEvents.AccountRegistered @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id);

            var account = Model.Account.Create(eventAudit, @event.Id);

            var projection = Projection.Create(eventAudit, account);
            await Projections.Create(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, AccountEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id);
            var account = (await Projections.Get(storeKey).ConfigureAwait(false)).Account;

            account.AddProject(eventAudit, @event.ProjectId, string.Empty);

            var projection = Projection.Create(eventAudit, account);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, AccountEvents.ProjectDeleted @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id);
            var account = (await Projections.Get(storeKey).ConfigureAwait(false)).Account;

            account.DeleteProject(eventAudit, @event.ProjectId);

            var projection = Projection.Create(eventAudit, account);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, ProjectEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.AccountId);
            var account = (await Projections.Get(storeKey).ConfigureAwait(false)).Account;

            account.SetProjectName(eventAudit, @event.Id, @event.Name);

            var projection = Projection.Create(eventAudit, account);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }
    }
}
