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

        public async Task Handle(ProjectEvents.ProjectCreated @event, CancellationToken stoppingToken)
        {
            var projection = ProjectDetailsDto.Create(@event.Id, @event.Name, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Create(ProjectDetailsDto.StoreKey(@event.Id), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.EnvironmentAdded @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddEnvironment(@event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.EnvironmentDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteEnvironment(@event.Key, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ToggleAdded @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddToggle(@event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ToggleDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteToggle(@event.Key, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ProjectDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(ProjectDetailsDto.StoreKey(@event.Id)).ConfigureAwait(false);
        }
    }
}