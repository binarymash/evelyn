namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<ToggleDetailsDto>
    {
        public ProjectionBuilder(IProjectionStore<ToggleDetailsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public override async Task HandleEvent(IEvent @event)
        {
            switch (@event)
            {
                case ToggleAdded toggleAdded:
                    await Handle(toggleAdded).ConfigureAwait(false);
                    break;
                case ToggleDeleted toggleDeleted:
                    await Handle(toggleDeleted).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }

        private async Task Handle(ToggleAdded @event)
        {
            try
            {
                var projection = new ToggleDetailsDto(@event.Id, @event.Version, @event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId);

                await Projections.AddOrUpdate(ToggleDetailsDto.StoreKey(@event.Id, @event.Key), projection);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(ToggleDeleted @event)
        {
            try
            {
                await Projections.Delete(ToggleDetailsDto.StoreKey(@event.Id, @event.Key));
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
