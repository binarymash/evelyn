namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class EventStreamHandler : EventStreamHandler<ToggleDetailsDto>
    {
        private readonly IProjectionStore<string, ToggleDetailsDto> _db;

        public EventStreamHandler(
            IProjectionStore<string, ToggleDetailsDto> db,
            IEventStreamFactory eventQueueFactory)
            : base(eventQueueFactory)
        {
            _db = db;
        }

        protected override async Task HandleEvent(IEvent @event)
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
                await _db.AddOrUpdate(GetStoreKey(@event.Id, @event.Key), projection);
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
                await _db.Delete(GetStoreKey(@event.Id, @event.Key));
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private string GetStoreKey(Guid projectId, string toggleKey)
        {
            return $"{projectId}-{toggleKey}";
        }
    }
}
