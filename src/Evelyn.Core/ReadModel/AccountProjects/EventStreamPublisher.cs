namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using AccountEvents = WriteModel.Account.Events;

    public class EventStreamPublisher :
        ICancellableEventHandler<AccountEvents.AccountRegistered>,
        ICancellableEventHandler<AccountEvents.ProjectCreated>,
        ICancellableEventHandler<AccountEvents.ProjectDeleted>
    {
        private readonly Queue<IEvent> _eventStream;

        public EventStreamPublisher(IEventStreamFactory eventStreamFactory)
        {
            _eventStream = eventStreamFactory.GetEventStream<AccountProjectsDto>();
        }

        public Task Handle(AccountEvents.AccountRegistered message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(AccountEvents.ProjectCreated message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task Handle(AccountEvents.ProjectDeleted message, CancellationToken token = default)
        {
            _eventStream.Enqueue(message);
            return Task.CompletedTask;
        }
    }
}
