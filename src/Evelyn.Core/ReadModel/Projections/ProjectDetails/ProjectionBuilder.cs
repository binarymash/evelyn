namespace Evelyn.Core.ReadModel.Projections.ProjectDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<ProjectDetailsDto>,
        IBuildProjectionsFrom<ProjectEvents.ProjectCreated>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentAdded>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentDeleted>,
        IBuildProjectionsFrom<ProjectEvents.ToggleAdded>,
        IBuildProjectionsFrom<ProjectEvents.ToggleDeleted>,
        IBuildProjectionsFrom<ProjectEvents.ProjectDeleted>
    {
        public ProjectionBuilder(IProjectionStore<ProjectDetailsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(long streamVersion, ProjectEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var projection = ProjectDetailsDto.Create(CreateEventAudit(streamVersion, @event), @event.Id, @event.Name);
            await Projections.Create(ProjectDetailsDto.StoreKey(@event.Id), projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.EnvironmentAdded @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddEnvironment(CreateEventAudit(streamVersion, @event), @event.Key, @event.Name);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.EnvironmentDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteEnvironment(CreateEventAudit(streamVersion, @event), @event.Key);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.ToggleAdded @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddToggle(CreateEventAudit(streamVersion, @event), @event.Key, @event.Name);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.ToggleDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteToggle(CreateEventAudit(streamVersion, @event), @event.Key);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.ProjectDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(ProjectDetailsDto.StoreKey(@event.Id)).ConfigureAwait(false);
        }
    }
}