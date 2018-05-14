namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using WriteModel.Project.Events;

    public class EventStreamPublisher
        : ICancellableEventHandler<ToggleAdded>,
          ICancellableEventHandler<ToggleDeleted>
    {
        private readonly Queue<IEvent> _eventStream;

        public EventStreamPublisher(IEventStreamFactory eventStreamFactory)
        {
            _eventStream = eventStreamFactory.GetEventStream<ToggleDetailsDto>();
        }

        public Task Handle(ToggleAdded message, CancellationToken token = default(CancellationToken))
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(ToggleDeleted message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }
    }
}
