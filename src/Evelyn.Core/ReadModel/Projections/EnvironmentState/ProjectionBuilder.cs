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
            var projection = EnvironmentStateDto.Create(CreateEventAudit(@event), toggleStates);
            await Projections.Create(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.EnvironmentStateDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ToggleStateAdded @event, CancellationToken stoppingToken)
        {
            var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            projection.AddToggleState(CreateEventAudit(@event), @event.ToggleKey, @event.Value);
            await Projections.Update(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ToggleStateChanged @event, CancellationToken stoppingToken)
        {
            var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            projection.ChangeToggleState(CreateEventAudit(@event), @event.ToggleKey, @event.Value);
            await Projections.Update(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ToggleStateDeleted @event, CancellationToken stoppingToken)
        {
            var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            projection.DeleteToggleState(CreateEventAudit(@event), @event.ToggleKey);
            await Projections.Update(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
        }
    }
}