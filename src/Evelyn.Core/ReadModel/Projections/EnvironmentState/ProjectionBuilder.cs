namespace Evelyn.Core.ReadModel.Projections.EnvironmentState
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<EnvironmentStateDto>,
        IBuildProjectionsFrom<EnvironmentStateAdded>,
        IBuildProjectionsFrom<EnvironmentStateDeleted>,
        IBuildProjectionsFrom<ToggleStateAdded>,
        IBuildProjectionsFrom<ToggleStateChanged>,
        IBuildProjectionsFrom<ToggleStateDeleted>
    {
        public ProjectionBuilder(IProjectionStore<EnvironmentStateDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(EnvironmentStateAdded @event, CancellationToken stoppingToken)
        {
            var toggleStates = @event.ToggleStates.Select(ts => new ToggleStateDto(ts.Key, ts.Value, @event.Version));
            var projection = new EnvironmentStateDto(@event.Version, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId, toggleStates);
            await Projections.AddOrUpdate(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }

        public async Task Handle(EnvironmentStateDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
        }

        public async Task Handle(ToggleStateAdded @event, CancellationToken stoppingToken)
        {
            var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            projection.AddToggleState(@event.ToggleKey, @event.Value, @event.Version, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }

        public async Task Handle(ToggleStateChanged @event, CancellationToken stoppingToken)
        {
            var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            projection.ChangeToggleState(@event.ToggleKey, @event.Value, @event.Version, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }

        public async Task Handle(ToggleStateDeleted @event, CancellationToken stoppingToken)
        {
            var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            projection.DeleteToggleState(@event.ToggleKey, @event.Version, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }
    }
}