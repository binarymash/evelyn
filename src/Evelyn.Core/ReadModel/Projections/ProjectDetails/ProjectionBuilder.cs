namespace Evelyn.Core.ReadModel.Projections.ProjectDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<Projection>,
        IBuildProjectionsFrom<ProjectEvents.ProjectCreated>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentAdded>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentDeleted>,
        IBuildProjectionsFrom<ProjectEvents.ToggleAdded>,
        IBuildProjectionsFrom<ProjectEvents.ToggleDeleted>,
        IBuildProjectionsFrom<ProjectEvents.ProjectDeleted>
    {
        public ProjectionBuilder(IProjectionStore<Projection> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(long streamVersion, ProjectEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamVersion, @event);
            var storeKey = Projection.StoreKey(@event.Id);

            var project = Model.Project.Create(eventAudit, @event.Id, @event.Name);

            var projection = Projection.Create(eventAudit, project);
            await Projections.Create(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.EnvironmentAdded @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamVersion, @event);
            var storeKey = Projection.StoreKey(@event.Id);
            var project = (await Projections.Get(storeKey).ConfigureAwait(false)).Project;

            project.AddEnvironment(eventAudit, @event.Key, @event.Name);

            var projection = Projection.Create(eventAudit, project);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.EnvironmentDeleted @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamVersion, @event);
            var storeKey = Projection.StoreKey(@event.Id);
            var project = (await Projections.Get(storeKey).ConfigureAwait(false)).Project;

            project.DeleteEnvironment(CreateEventAudit(streamVersion, @event), @event.Key);

            var projection = Projection.Create(eventAudit, project);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.ToggleAdded @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamVersion, @event);
            var storeKey = Projection.StoreKey(@event.Id);
            var project = (await Projections.Get(storeKey).ConfigureAwait(false)).Project;

            project.AddToggle(CreateEventAudit(streamVersion, @event), @event.Key, @event.Name);

            var projection = Projection.Create(eventAudit, project);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.ToggleDeleted @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamVersion, @event);
            var storeKey = Projection.StoreKey(@event.Id);
            var project = (await Projections.Get(storeKey).ConfigureAwait(false)).Project;

            project.DeleteToggle(CreateEventAudit(streamVersion, @event), @event.Key);

            var projection = Projection.Create(eventAudit, project);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.ProjectDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(Projection.StoreKey(@event.Id)).ConfigureAwait(false);
        }
    }
}