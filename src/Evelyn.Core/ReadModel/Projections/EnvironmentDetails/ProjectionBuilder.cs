namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<EnvironmentDetailsDto>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentAdded>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentDeleted>
    {
        public ProjectionBuilder(IProjectionStore<EnvironmentDetailsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(ProjectEvents.EnvironmentAdded @event, CancellationToken stoppingToken)
        {
            var projection = new EnvironmentDetailsDto(@event.Id, @event.Version, @event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId);
            await Projections.AddOrUpdate(EnvironmentDetailsDto.StoreKey(@event.Id, @event.Key), projection).ConfigureAwait(false);
        }

        public async Task Handle(ProjectEvents.EnvironmentDeleted @event, CancellationToken stoppingToken)
        {
            await Projections.Delete(EnvironmentDetailsDto.StoreKey(@event.Id, @event.Key)).ConfigureAwait(false);
        }
    }
}