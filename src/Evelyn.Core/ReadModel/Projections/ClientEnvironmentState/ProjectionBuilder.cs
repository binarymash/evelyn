namespace Evelyn.Core.ReadModel.Projections.ClientEnvironmentState
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<Projection>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentStateAdded>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentStateDeleted>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateAdded>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateChanged>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateDeleted>
    {
        public ProjectionBuilder(IProjectionStore<Projection> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(long streamPosition, ProjectEvents.EnvironmentStateAdded @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.EnvironmentKey);

            var toggleStates = @event.ToggleStates.Select(ts => Model.ToggleState.Create(ts.Key, ts.Value));
            var environmentState = Model.EnvironmentState.Create(eventAudit, toggleStates);

            var projection = Projection.Create(eventAudit, environmentState);
            await Projections.Create(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, ProjectEvents.EnvironmentStateDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = Projection.StoreKey(@event.Id, @event.EnvironmentKey);

            await Projections.Delete(storeKey).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, ProjectEvents.ToggleStateAdded @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.EnvironmentKey);
            var environmentState = (await Projections.Get(storeKey).ConfigureAwait(false)).EnvironmentState;

            environmentState.AddToggleState(eventAudit, @event.ToggleKey, @event.Value);

            var projection = Projection.Create(eventAudit, environmentState);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, ProjectEvents.ToggleStateChanged @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.EnvironmentKey);
            var environmentState = (await Projections.Get(storeKey).ConfigureAwait(false)).EnvironmentState;

            environmentState.ChangeToggleState(CreateEventAudit(streamPosition, @event), @event.ToggleKey, @event.Value);

            var projection = Projection.Create(eventAudit, environmentState);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, ProjectEvents.ToggleStateDeleted @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.EnvironmentKey);
            var environmentState = (await Projections.Get(storeKey).ConfigureAwait(false)).EnvironmentState;

            environmentState.DeleteToggleState(CreateEventAudit(streamPosition, @event), @event.ToggleKey);

            var projection = Projection.Create(eventAudit, environmentState);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }
    }
}