namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using WriteModel.Project.Events;

    public class EventStreamPublisher :
        ICancellableEventHandler<ProjectCreated>,
        ICancellableEventHandler<EnvironmentAdded>,
        ICancellableEventHandler<EnvironmentDeleted>,
        ICancellableEventHandler<ToggleAdded>,
        ICancellableEventHandler<ToggleDeleted>
    {
        private readonly Queue<IEvent> _eventStream;

        public EventStreamPublisher(IEventStreamFactory eventStreamFactory)
        {
            _eventStream = eventStreamFactory.GetEventStream<ProjectDetailsDto>();
        }

        public Task Handle(ProjectCreated message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(EnvironmentAdded message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(ToggleAdded message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(EnvironmentDeleted message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(ToggleDeleted message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(ProjectDeleted message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }
    }
}
