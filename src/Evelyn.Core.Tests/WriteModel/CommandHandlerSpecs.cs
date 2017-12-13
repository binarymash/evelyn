namespace Evelyn.Core.Tests.WriteModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using CQRSlite.Events;
    using CQRSlite.Snapshotting;
    using Shouldly;

    public abstract class CommandHandlerSpecs<TAggregate, THandler, TCommand>
        where TAggregate : AggregateRoot
        where THandler : class
        where TCommand : ICommand
    {
        private dynamic _handler;
        private Snapshot _snapshot;
        private SpecSnapShotStorage _snapshotStore;
        private SpecEventPublisher _eventPublisher;
        private SpecEventStorage _eventStorage;

        public CommandHandlerSpecs()
        {
            HistoricalEvents = new List<IEvent>();
        }

        protected TAggregate Aggregate { get; set; }

        protected ISession Session { get; set; }

        protected IList<IEvent> HistoricalEvents { get; set; }

        protected IList<IEvent> PublishedEvents { get; set; }

        protected IList<IEvent> EventDescriptors { get; set; }

        protected Exception ThrownException { get; private set; }

        protected abstract THandler BuildHandler();

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
#pragma warning disable SA1129 // Do not use default value type constructor
                    ((ICancellableCommandHandler<TCommand>)_handler).Handle(command, new CancellationToken()).GetAwaiter().GetResult();
#pragma warning restore SA1129 // Do not use default value type constructor
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
