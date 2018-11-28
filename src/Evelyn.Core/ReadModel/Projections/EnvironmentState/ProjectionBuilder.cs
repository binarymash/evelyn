namespace Evelyn.Core.ReadModel.Projections.EnvironmentState
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<EnvironmentStateDto>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentStateAdded>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentStateDeleted>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateAdded>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateChanged>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateDeleted>
    {
        public ProjectionBuilder(IProjectionStore<EnvironmentStateDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(ProjectEvents.EnvironmentStateAdded @event, CancellationToken stoppingToken)
        {
            var toggleStates = @event.ToggleStates.Select(ts => new ToggleStateDto(ts.Key, ts.Value, @event.Version));
            var projection = EnvironmentStateDto.Create(toggleStates, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Create(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.EnvironmentStateDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ToggleStateAdded @event, CancellationToken stoppingToken)
        {
            var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            projection.AddToggleState(@event.ToggleKey, @event.Value, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Update(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ToggleStateChanged @event, CancellationToken stoppingToken)
        {
            var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            projection.ChangeToggleState(@event.ToggleKey, @event.Value, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Update(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ToggleStateDeleted @event, CancellationToken stoppingToken)
        {
            var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            projection.DeleteToggleState(@event.ToggleKey, @event.OccurredAt, @event.UserId, @event.Version);
            await Projections.Update(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }
    }
}