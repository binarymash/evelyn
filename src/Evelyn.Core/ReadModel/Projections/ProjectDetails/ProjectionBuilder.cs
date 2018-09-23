namespace Evelyn.Core.ReadModel.Projections.ProjectDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using Evelyn.Core.WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<ProjectDetailsDto>,
        IBuildProjectionsFrom<ProjectCreated>,
        IBuildProjectionsFrom<EnvironmentAdded>,
        IBuildProjectionsFrom<EnvironmentDeleted>,
        IBuildProjectionsFrom<ToggleAdded>,
        IBuildProjectionsFrom<ToggleDeleted>,
        IBuildProjectionsFrom<ProjectDeleted>
    {
        public ProjectionBuilder(IProjectionStore<ProjectDetailsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(ProjectCreated @event, CancellationToken stoppingToken)
        {
            var projection = new ProjectDetailsDto(@event.Id, @event.Name, null, null, @event.Version, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(ProjectDetailsDto.StoreKey(@event.Id), projection).ConfigureAwait(false);
        }

        public async Task Handle(EnvironmentAdded @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddEnvironment(@event.Key, @event.Name, @event.OccurredAt, @event.Version, @event.UserId);
            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(EnvironmentDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteEnvironment(@event.Key, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ToggleAdded @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddToggle(@event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ToggleDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = ProjectDetailsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.DeleteToggle(@event.Key, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(ProjectDetailsDto.StoreKey(@event.Id)).ConfigureAwait(false);
        }
    }
}