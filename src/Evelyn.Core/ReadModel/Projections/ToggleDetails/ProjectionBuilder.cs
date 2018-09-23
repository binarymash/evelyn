namespace Evelyn.Core.ReadModel.Projections.ToggleDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<ToggleDetailsDto>,
        IBuildProjectionsFrom<ToggleAdded>,
        IBuildProjectionsFrom<ToggleDeleted>
    {
        public ProjectionBuilder(IProjectionStore<ToggleDetailsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(ToggleAdded @event, CancellationToken stoppingToken)
        {
            var projection = new ToggleDetailsDto(@event.Id, @event.Version, @event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(ToggleDetailsDto.StoreKey(@event.Id, @event.Key), projection).ConfigureAwait(false);
        }

        public async Task Handle(ToggleDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(ToggleDetailsDto.StoreKey(@event.Id, @event.Key)).ConfigureAwait(false);
        }
    }
}
