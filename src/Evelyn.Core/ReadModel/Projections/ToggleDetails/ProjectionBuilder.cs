namespace Evelyn.Core.ReadModel.Projections.ToggleDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<Projection>,
        IBuildProjectionsFrom<ProjectEvents.ToggleAdded>,
        IBuildProjectionsFrom<ProjectEvents.ToggleDeleted>
    {
        public ProjectionBuilder(IProjectionStore<Projection> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(long streamVersion, ProjectEvents.ToggleAdded @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamVersion, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.Key);

            var toggle = Model.Toggle.Create(eventAudit, @event.Id, @event.Key, @event.Name);

            var projection = Projection.Create(eventAudit, toggle);
            await Projections.Create(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.ToggleDeleted @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamVersion, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.Key);

            await Projections.Delete(storeKey).ConfigureAwait(false);
        }
    }
}
