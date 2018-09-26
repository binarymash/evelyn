namespace Evelyn.Core.ReadModel.Projections.ToggleDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<ToggleDetailsDto>,
        IBuildProjectionsFrom<ProjectEvents.ToggleAdded>,
        IBuildProjectionsFrom<ProjectEvents.ToggleDeleted>
    {
        public ProjectionBuilder(IProjectionStore<ToggleDetailsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(ProjectEvents.ToggleAdded @event, CancellationToken stoppingToken)
        {
            var projection = new ToggleDetailsDto(@event.Id, @event.Version, @event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId);
            await Projections.Create(ToggleDetailsDto.StoreKey(@event.Id, @event.Key), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.ToggleDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(ToggleDetailsDto.StoreKey(@event.Id, @event.Key)).ConfigureAwait(false);
        }
    }
}
