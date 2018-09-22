namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<EnvironmentDetailsDto>
    {
        public ProjectionBuilder(IProjectionStore<EnvironmentDetailsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public override async Task HandleEvent(IEvent @event)
        {
            switch (@event)
            {
                case EnvironmentAdded ea:
                    await Handle(ea).ConfigureAwait(false);
                    break;
                case EnvironmentDeleted ed:
                    await Handle(ed).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }

        private async Task Handle(EnvironmentAdded @event)
        {
            try
            {
                var projection = new EnvironmentDetailsDto(@event.Id, @event.Version, @event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId);
                await Projections.AddOrUpdate(EnvironmentDetailsDto.StoreKey(@event.Id, @event.Key), projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(EnvironmentDeleted @event)
        {
            try
            {
                await Projections.Delete(EnvironmentDetailsDto.StoreKey(@event.Id, @event.Key)).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
