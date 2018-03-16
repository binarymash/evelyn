namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using WriteModel.Project.Events;

    public class EventStreamPublisher
        : ICancellableEventHandler<ToggleAdded>
    {
        private readonly Queue<IEvent> _eventStream;

        public EventStreamPublisher(IEventStreamFactory eventStreamFactory)
        {
            _eventStream = eventStreamFactory.GetEventStream<ToggleDetailsDto>();
        }

        public Task Handle(ToggleAdded message, CancellationToken token)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }
    }
}
