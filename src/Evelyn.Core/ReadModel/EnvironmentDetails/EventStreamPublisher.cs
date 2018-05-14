namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using WriteModel.Project.Events;

    public class EventStreamPublisher :
        ICancellableEventHandler<EnvironmentAdded>,
        ICancellableEventHandler<EnvironmentDeleted>
    {
        private readonly Queue<IEvent> _eventStream;

        public EventStreamPublisher(IEventStreamFactory eventStreamFactory)
        {
            _eventStream = eventStreamFactory.GetEventStream<EnvironmentDetailsDto>();
        }

        public Task Handle(EnvironmentAdded message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(EnvironmentDeleted message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }
    }
}
