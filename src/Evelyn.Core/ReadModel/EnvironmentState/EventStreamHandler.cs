namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class EventStreamHandler : EventStreamHandler<EnvironmentStateDto>
    {
        private IProjectionStore<string, EnvironmentStateDto> _db;

        public EventStreamHandler(
            IProjectionStore<string, EnvironmentStateDto> db,
            IEventStreamFactory eventQueueFactory)
            : base(eventQueueFactory)
        {
            _db = db;
        }

        protected override async Task HandleEvent(IEvent @event)
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
                var dto = new EnvironmentStateDto(@event.Version, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId, toggleStates);
                await _db.AddOrUpdate(StoreKey(@event.Id, @event.EnvironmentKey), dto).ConfigureAwait(false);
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
                await _db.Delete(StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
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
                var dto = await _db.Get(StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
                dto.AddToggleState(@event.ToggleKey, @event.Value, @event.Version, @event.OccurredAt, @event.UserId);
                await _db.AddOrUpdate(StoreKey(@event.Id, @event.EnvironmentKey), dto).ConfigureAwait(false);
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
                var dto = await _db.Get(StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
                dto.ChangeToggleState(@event.ToggleKey, @event.Value, @event.Version, @event.OccurredAt, @event.UserId);
                await _db.AddOrUpdate(StoreKey(@event.Id, @event.EnvironmentKey), dto).ConfigureAwait(false);
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
                var dto = await _db.Get(StoreKey(@event.Id, @event.EnvironmentKey)).ConfigureAwait(false);
                dto.DeleteToggleState(@event.ToggleKey, @event.Version, @event.OccurredAt, @event.UserId);
                await _db.AddOrUpdate(StoreKey(@event.Id, @event.EnvironmentKey), dto).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{projectId}-{environmentKey}";
        }
    }
}
