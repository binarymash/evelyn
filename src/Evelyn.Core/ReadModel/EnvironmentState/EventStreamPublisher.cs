namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using WriteModel.Project.Events;

    public class EventStreamPublisher :
          ICancellableEventHandler<EnvironmentAdded>,
          ICancellableEventHandler<ToggleAdded>,
          ICancellableEventHandler<ToggleStateChanged>
    {
        private readonly Queue<IEvent> _eventStream;

        public EventStreamPublisher(IEventStreamFactory eventStreamFactory)
        {
            _eventStream = eventStreamFactory.GetEventStream<EnvironmentStateDto>();
        }

        public Task Handle(EnvironmentAdded message, CancellationToken token = default(CancellationToken))
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(ToggleAdded message, CancellationToken token = default(CancellationToken))
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(ToggleStateChanged message, CancellationToken token = default(CancellationToken))
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }
    }
}
