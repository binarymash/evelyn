namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<Projection>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentAdded>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentDeleted>
    {
        public ProjectionBuilder(IProjectionStore<Projection> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(long streamPosition, ProjectEvents.EnvironmentAdded @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.Key);

            var environment = Model.Environment.Create(eventAudit, @event.Id, @event.Key, @event.Name);

            var projection = Projection.Create(eventAudit, environment);
            await Projections.Create(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, ProjectEvents.EnvironmentDeleted @event, CancellationToken stoppingToken)
        {
            var storeKey = Projection.StoreKey(@event.Id, @event.Key);

            await Projections.Delete(storeKey).ConfigureAwait(false);
        }
    }
}