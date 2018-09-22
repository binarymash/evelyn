namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<EnvironmentStateDto>
    {
        public ProjectionBuilder(IProjectionStore<EnvironmentStateDto> projectionStore)
            : base(projectionStore)
        {
        }

        public override async Task HandleEvent(IEvent @event)
        {
            switch (@event)
            {
                case EnvironmentStateAdded esa:
                    await Handle(esa).ConfigureAwait(false);
                    break;
                case EnvironmentStateDeleted esd:
                    await Handle(esd).ConfigureAwait(false);
                    break;
                case ToggleStateAdded tsa:
                    await Handle(tsa).ConfigureAwait(false);
                    break;
                case ToggleStateChanged tsc:
                    await Handle(tsc).ConfigureAwait(false);
                    break;
                case ToggleStateDeleted tsd:
                    await Handle(tsd).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }

        private async Task Handle(EnvironmentStateAdded @event)
        {
            try
            {
                var toggleStates = @event.ToggleStates.Select(ts => new ToggleStateDto(ts.Key, ts.Value, @event.Version));
                var projection = new EnvironmentStateDto(@event.Version, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId, toggleStates);

                await Projections.AddOrUpdate(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(EnvironmentStateDeleted @event)
        {
            try
            {
                await Projections.Delete(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(ToggleStateAdded @event)
        {
            try
            {
                var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);

                projection.AddToggleState(@event.ToggleKey, @event.Value, @event.Version, @event.OccurredAt, @event.UserId);

                await Projections.AddOrUpdate(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(ToggleStateChanged @event)
        {
            try
            {
                var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);

                projection.ChangeToggleState(@event.ToggleKey, @event.Value, @event.Version, @event.OccurredAt, @event.UserId);

                await Projections.AddOrUpdate(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(ToggleStateDeleted @event)
        {
            try
            {
                var projection = await Projections.Get(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);

                projection.DeleteToggleState(@event.ToggleKey, @event.Version, @event.OccurredAt, @event.UserId);

                await Projections.AddOrUpdate(EnvironmentStateDto.StoreKey(@event.Id, @event.EnvironmentKey), projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
