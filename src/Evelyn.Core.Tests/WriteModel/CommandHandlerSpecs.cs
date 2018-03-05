namespace Evelyn.Core.Tests.WriteModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.WriteModel.Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using CQRSlite.Events;
    using FluentAssertions;

    public abstract class CommandHandlerSpecs<TAggregate, THandler, TCommand>
        where TAggregate : AggregateRoot
        where THandler : class
        where TCommand : ICommand
    {
        private dynamic _handler;
        private SpecEventPublisher _eventPublisher;
        private SpecEventStore _eventStore;

        protected CommandHandlerSpecs()
        {
            HistoricalEvents = new List<IEvent>();
            DataFixture = new Fixture();
            UserId = DataFixture.Create<string>();
        }

        protected string UserId { get; set; }

        protected Fixture DataFixture { get; }

        protected TAggregate Aggregate { get; set; }

        protected ISession Session { get; set; }

        protected IList<IEvent> HistoricalEvents { get; set; }

        protected IList<IEvent> PublishedEvents { get; set; }

        protected IList<IEvent> EventDescriptors { get; set; }

        protected Exception ThrownException { get; private set; }

        protected abstract THandler BuildHandler();

        protected void WhenWeHandle(TCommand command)
        {
            var aggregateId = ExtractAggregateId(command);

            _eventPublisher = new SpecEventPublisher();
            _eventStore = new SpecEventStore(_eventPublisher, HistoricalEvents);
            var repository = new Repository(_eventStore);
            Session = new Session(repository);
            Aggregate = GetAggregate(aggregateId).Result;

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

            PublishedEvents = _eventPublisher.PublishedEvents;
            EventDescriptors = _eventStore.Events;
        }

        protected void ThenNoEventIsPublished()
        {
            PublishedEvents.Count.Should().Be(0);
        }

        protected void ThenOneEventIsPublished()
        {
            PublishedEvents.Count.Should().Be(1);
        }

        protected void ThenAnInvalidOperationExceptionIsThrownWithMessage(string expectedMessage)
        {
            ThrownException.Should().BeOfType<InvalidOperationException>();
            ThrownException.Message.Should().Be(expectedMessage);
        }

        private Guid ExtractAggregateId(TCommand command)
        {
            if (command is CreateProject)
            {
                return (command as CreateProject).Id;
            }

            if (command is AddEnvironment)
            {
                return (command as AddEnvironment).ProjectId;
            }

            if (command is AddToggle)
            {
                return (command as AddToggle).ProjectId;
            }

            if (command is ChangeToggleState)
            {
                return (command as ChangeToggleState).ProjectId;
            }

            return Guid.Empty;
        }

        private async Task<TAggregate> GetAggregate(Guid id)
        {
            try
            {
                var aggregate = await Session.Get<TAggregate>(id);
                return aggregate;
            }
            catch (AggregateNotFoundException)
            {
                return null;
            }
        }
    }
}
