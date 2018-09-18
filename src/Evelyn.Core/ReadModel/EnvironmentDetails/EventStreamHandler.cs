namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class EventStreamHandler : EventStreamHandler<EnvironmentDetailsDto>
    {
        private readonly IProjectionStore<string, EnvironmentDetailsDto> _db;

        public EventStreamHandler(
            IProjectionStore<string, EnvironmentDetailsDto> db,
            IEventStreamFactory eventQueueFactory)
            : base(eventQueueFactory)
        {
            _db = db;
        }

        protected override async Task HandleEvent(IEvent @event)
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
                var dto = new EnvironmentDetailsDto(@event.Id, @event.Version, @event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId);
                await _db.AddOrUpdate(StoreKey(@event.Id, @event.Key), dto).ConfigureAwait(false);
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
                await _db.Delete(StoreKey(@event.Id, @event.Key)).ConfigureAwait(false);
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
