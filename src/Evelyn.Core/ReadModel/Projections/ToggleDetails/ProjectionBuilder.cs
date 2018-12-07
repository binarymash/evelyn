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

        public async Task Handle(long streamVersion, ProjectEvents.ToggleAdded @event, CancellationToken stoppingToken)
        {
            var projection = ToggleDetailsDto.Create(CreateEventAudit(streamVersion, @event), @event.Id, @event.Key, @event.Name);
            await Projections.Create(ToggleDetailsDto.StoreKey(@event.Id, @event.Key), projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamVersion, ProjectEvents.ToggleDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(ToggleDetailsDto.StoreKey(@event.Id, @event.Key)).ConfigureAwait(false);
        }
    }
}
