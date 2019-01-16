namespace Evelyn.Core.ReadModel.Projections.ToggleState
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<Projection>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateAdded>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateChanged>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateDeleted>
    {
        public ProjectionBuilder(IProjectionStore<Projection> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(long streamPosition, ProjectEvents.ToggleStateAdded @event, CancellationToken stoppingToken)
        {
            Model.ToggleState toggleState;
            bool toggleWasCreated = false;
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.ToggleKey);

            try
            {
                toggleState = (await Projections.Get(storeKey).ConfigureAwait(false)).ToggleState;
            }
            catch (ProjectionNotFoundException)
            {
                // TODO: fix this so we don't have to catch the an exception!
                toggleState = Model.ToggleState.Create(eventAudit);
                toggleWasCreated = true;
            }

            toggleState.AddEnvironmentState(eventAudit, @event.EnvironmentKey, @event.Value);
            var projection = Projection.Create(eventAudit, toggleState);

            if (toggleWasCreated)
            {
                await Projections.Create(storeKey, projection).ConfigureAwait(false);
            }
            else
            {
                await Projections.Update(storeKey, projection).ConfigureAwait(false);
            }
        }

        public async Task Handle(long streamPosition, ProjectEvents.ToggleStateChanged @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.ToggleKey);
            var toggleState = (await Projections.Get(storeKey).ConfigureAwait(false)).ToggleState;

            toggleState.ChangeEnvironmentState(eventAudit, @event.EnvironmentKey, @event.Value);

            var projection = Projection.Create(eventAudit, toggleState);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, ProjectEvents.ToggleStateDeleted @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.ToggleKey);
            var toggleState = (await Projections.Get(storeKey).ConfigureAwait(false)).ToggleState;

            toggleState.DeleteEnvironmentState(eventAudit, @event.EnvironmentKey);

            var projection = Projection.Create(eventAudit, toggleState);
            if (projection.ToggleState.EnvironmentStates.Any())
            {
                await Projections.Update(storeKey, projection).ConfigureAwait(false);
            }
            else
            {
                await Projections.Delete(storeKey).ConfigureAwait(false);
            }
        }
    }
}