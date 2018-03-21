namespace Evelyn.Core.Tests.WriteModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.WriteModel;
    using Core.WriteModel.Account.Commands;
    using Core.WriteModel.Project.Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using CQRSlite.Events;
    using FluentAssertions;
    using KellermanSoftware.CompareNetObjects;
    using Newtonsoft.Json;

    public abstract class CommandHandlerSpecs<TAggregate, THandler, TCommand>
        where TAggregate : EvelynAggregateRoot
        where THandler : class
        where TCommand : ICommand
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly CompareLogic _compareLogic;
        private dynamic _handler;
        private SpecEventPublisher _eventPublisher;
        private SpecEventStore _eventStore;

        protected CommandHandlerSpecs()
        {
            HistoricalEvents = new List<IEvent>();
            DataFixture = new Fixture();
            UserId = DataFixture.Create<string>();
            var comparisonConfig = new ComparisonConfig
            {
                MaxDifferences = int.MaxValue,
            };
            _compareLogic = new CompareLogic(comparisonConfig);

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };
        }

        protected string UserId { get; set; }

        protected Fixture DataFixture { get; }

        protected ISession Session { get; set; }

        protected IList<IEvent> HistoricalEvents { get; set; }

        protected IList<IEvent> PublishedEvents { get; set; }

        protected IList<IEvent> EventDescriptors { get; set; }

        protected Exception ThrownException { get; private set; }

        protected TAggregate OriginalAggregate { get; private set; }

        protected TAggregate NewAggregate { get; private set; }

        protected DateTimeOffset TimeBeforeHandling { get; private set; }

        protected DateTimeOffset TimeAfterHandling { get; private set; }

        protected ComparisonResult ComparisonResult { get; private set; }

        protected abstract THandler BuildHandler();

        protected void WhenWeHandle(TCommand command)
        {
            var aggregateId = ExtractAggregateId(command);

            _eventPublisher = new SpecEventPublisher();
            _eventStore = new SpecEventStore(_eventPublisher, HistoricalEvents);
            var repository = new Repository(_eventStore);

            Session = new Session(repository);
            OriginalAggregate = GetCopyOfAggregate(aggregateId);

            _handler = BuildHandler();

            try
            {
                TimeBeforeHandling = DateTimeOffset.UtcNow;

                if (_handler is ICancellableCommandHandler<TCommand>)
                {
                    ((ICancellableCommandHandler<TCommand>)_handler).Handle(command).GetAwaiter().GetResult();
                }
                else if (_handler is ICommandHandler<TCommand>)
                {
                    ((ICommandHandler<TCommand>)_handler).Handle(command).GetAwaiter().GetResult();
                }
                else
                {
                    throw new InvalidCastException(
                        $"{nameof(_handler)} is not a command handler of type {typeof(TCommand)}");
                }

                TimeAfterHandling = DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
            finally
            {
                NewAggregate = GetAggregate(aggregateId).Result;
                ComparisonResult = _compareLogic.Compare(OriginalAggregate, NewAggregate);
                PublishedEvents = _eventPublisher.PublishedEvents;
                EventDescriptors = _eventStore.Events;
            }
        }

        protected void ThenNoEventIsPublished()
        {
            PublishedEvents.Count.Should().Be(0);
        }

        protected void ThenOneEventIsPublished()
        {
            PublishedEvents.Count.Should().Be(1);
        }

        protected void ThenTwoEventsArePublished()
        {
            PublishedEvents.Count.Should().Be(2);
        }

        protected void ThenThreeEventsArePublished()
        {
            PublishedEvents.Count.Should().Be(3);
        }

        protected void ThenAnInvalidOperationExceptionIsThrownWithMessage(string expectedMessage)
        {
            ThrownException.Should().BeOfType<InvalidOperationException>();
            ThrownException.Message.Should().Be(expectedMessage);
        }

        protected void ThenThereAreNoChangesOnTheAggregate()
        {
            ComparisonResult.AreEqual.Should().BeTrue();
        }

        protected void ThenThereAreFourChangesOnTheAggregate()
        {
            ComparisonResult.Differences.Count.Should().Be(4);
        }

        protected void ThenThereAreFiveChangesOnTheAggregate()
        {
            ComparisonResult.Differences.Count.Should().Be(5);
        }

        protected void ThenThereAreEightChangesOnTheAggregate()
        {
            ComparisonResult.Differences.Count.Should().Be(8);
        }

        protected void ThenThereAreTwelveChangesOnTheAggregate()
        {
            ComparisonResult.Differences.Count.Should().Be(12);
        }

        protected void ThenTheAggregateRootVersionHasBeenIncreasedByOne()
        {
            NewAggregate.Version.Should().Be(OriginalAggregate.Version + 1);
        }

        protected void ThenTheAggregateRootVersionHasBeenIncreasedByTwo()
        {
            NewAggregate.Version.Should().Be(OriginalAggregate.Version + 2);
        }

        protected void ThenTheAggregateRootVersionHasBeenIncreasedByThree()
        {
            NewAggregate.Version.Should().Be(OriginalAggregate.Version + 3);
        }

        protected void ThenTheAggregateRootLastModifiedTimeHasBeenUpdated()
        {
            NewAggregate.LastModified.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        protected void ThenTheAggregateRootLastModifiedByHasBeenUpdated()
        {
            NewAggregate.LastModifiedBy.Should().Be(UserId);
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

        private TAggregate GetCopyOfAggregate(Guid id)
        {
            var serialized = JsonConvert.SerializeObject(GetAggregate(id).Result);
            return JsonConvert.DeserializeObject<TAggregate>(serialized, _serializerSettings);
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
