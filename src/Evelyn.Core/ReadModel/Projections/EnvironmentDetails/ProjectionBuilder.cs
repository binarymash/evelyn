namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<EnvironmentDetailsDto>,
        IBuildProjectionsFrom<EnvironmentAdded>,
        IBuildProjectionsFrom<EnvironmentDeleted>
    {
        public ProjectionBuilder(IProjectionStore<EnvironmentDetailsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(EnvironmentAdded @event, CancellationToken stoppingToken)
        {
            var projection = new EnvironmentDetailsDto(@event.Id, @event.Version, @event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(EnvironmentDetailsDto.StoreKey(@event.Id, @event.Key), projection).ConfigureAwait(false);
        }

        public async Task Handle(EnvironmentDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(EnvironmentDetailsDto.StoreKey(@event.Id, @event.Key)).ConfigureAwait(false);
        }
    }
}