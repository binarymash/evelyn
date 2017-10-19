namespace Evelyn.Core.Tests
{
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using CQRSlite.Events;
    using CQRSlite.Snapshotting;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class CommandHandlerSpecs<TAggregate, THandler, TCommand>
        where TAggregate : AggregateRoot
        where THandler : class
        where TCommand : ICommand
    {
        protected TAggregate Aggregate { get; set; }
        //protected TCommand Command { get; set; }
        protected ISession Session { get; set; }
        protected IList<IEvent> HistoricalEvents { get; set; }
        protected IList<IEvent> PublishedEvents { get; set; }
        protected IList<IEvent> EventDescriptors { get; set; }
        private dynamic _handler;
        private Snapshot _snapshot;
        private SpecSnapShotStorage _snapshotStore;
        private SpecEventPublisher _eventPublisher;
        private SpecEventStorage _eventStorage;
        protected abstract THandler BuildHandler();
        protected Exception ThrownException { get; private set; }

        public CommandHandlerSpecs()
        {
            HistoricalEvents = new List<IEvent>();
        }

        protected void WhenWeHandle(TCommand command)
        {
            _eventPublisher = new SpecEventPublisher();
            _eventStorage = new SpecEventStorage(_eventPublisher, HistoricalEvents);
            _snapshotStore = new SpecSnapShotStorage(_snapshot);

            var snapshotStrategy = new DefaultSnapshotStrategy();
            var repository = new SnapshotRepository(_snapshotStore, snapshotStrategy, new Repository(_eventStorage), _eventStorage);
            Session = new Session(repository);
            Aggregate = GetAggregate().Result;

            _handler = BuildHandler();

            try
            {
                if (_handler is ICancellableCommandHandler<TCommand>)
                {
                    ((ICancellableCommandHandler<TCommand>)_handler).Handle(command, new CancellationToken()).GetAwaiter().GetResult();
                }
                else if (_handler is ICommandHandler<TCommand>)
                {
                    ((ICommandHandler<TCommand>)_handler).Handle(command).GetAwaiter().GetResult();
                }
                else
                {
                    throw new InvalidCastException($"{nameof(_handler)} is not a command handler of type {typeof(TCommand)}");
                }
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }

            _snapshot = _snapshotStore.Snapshot;
            PublishedEvents = _eventPublisher.PublishedEvents;
            EventDescriptors = _eventStorage.Events;
        }

        protected void ThenNoEventIsPublished()
        {
            PublishedEvents.Count.ShouldBe(0);
        }

        protected void ThenOneEventIsPublished()
        {
            PublishedEvents.Count.ShouldBe(1);
        }

        protected void ThenAnInvalidOperationExceptionIsThrownWithMessage(string expectedMessage)
        {
            ThrownException.ShouldBeOfType<InvalidOperationException>();
            ThrownException.Message.ShouldBe(expectedMessage);
        }

        private async Task<TAggregate> GetAggregate()
        {
            try
            {
                return await Session.Get<TAggregate>(Guid.Empty);
            }
            catch (AggregateNotFoundException)
            {
                return null;
            }
        }
    }
}
