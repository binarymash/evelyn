namespace Evelyn.Core.Tests.WriteModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.WriteModel;
    using Core.WriteModel.Project.Domain;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using CQRSlite.Events;
    using FluentAssertions;
    using KellermanSoftware.CompareNetObjects;
    using Newtonsoft.Json;

    using AccountCommands = Core.WriteModel.Account.Commands;
    using Environment = global::Evelyn.Core.WriteModel.Project.Domain.Environment;
    using EvelynCommands = Core.WriteModel.Evelyn.Commands;
    using ProjectCommands = Core.WriteModel.Project.Commands;

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
                IgnoreCollectionOrder = true,
                CollectionMatchingSpec = new Dictionary<Type, IEnumerable<string>>
                {
                    { typeof(Environment), new[] { "Key" } },
                    { typeof(Toggle), new[] { "Key" } },
                    { typeof(EnvironmentState), new[] { "EnvironmentKey" } },
                    { typeof(ToggleState), new[] { "Key" } }
                }
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

        protected bool IgnoreCollectionOrderDuringComparison { get; set; }

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

                _compareLogic.Config.IgnoreCollectionOrder = IgnoreCollectionOrderDuringComparison;
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

        protected void ThenSevenEventsArePublished()
        {
            PublishedEvents.Count.Should().Be(7);
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

        protected void ThenTheNumberOfChangesOnTheAggregateIs(int numberOfChanges)
        {
            ComparisonResult.Differences.Count.Should().Be(numberOfChanges);
        }

        protected void ThenTheAggregateRootVersionHasBeenIncreasedBy(int increment)
        {
            NewAggregate.Version.Should().Be(OriginalAggregate.Version + increment);
        }

        protected void ThenTheAggregateRootVersionIsZero()
        {
            NewAggregate.Version.Should().Be(0);
        }

        protected void ThenTheAggregateRootVersionIsOne()
        {
            NewAggregate.Version.Should().Be(1);
        }

        protected void ThenTheAggregateRootCreatedTimeHasBeenSet()
        {
            NewAggregate.Created.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        protected void ThenTheAggregateRootCreatedByHasBeenSet()
        {
            NewAggregate.LastModifiedBy.Should().Be(UserId);
        }

        protected void ThenTheAggregateRootLastModifiedTimeHasBeenUpdated()
        {
            NewAggregate.LastModified.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        protected void ThenTheAggregateRootLastModifiedByHasBeenUpdated()
        {
            NewAggregate.LastModifiedBy.Should().Be(UserId);
        }

        protected void ThenAConcurrencyExceptionIsThrown()
        {
            ThrownException.Should().BeOfType<ConcurrencyException>();
        }

        /// <summary>
        /// TODO: This stinks!
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>The aggregate root that the command is for</returns>
        private Guid ExtractAggregateId(TCommand command)
        {
            switch (command as ICommand)
            {
                case AccountCommands.CreateProject.Command c:
                    return c.Id;

                case EvelynCommands.CreateSystem.Command c:
                    return Constants.EvelynSystem;
                case EvelynCommands.RegisterAccount.Command c:
                    return Constants.EvelynSystem;
                case EvelynCommands.StartSystem.Command c:
                    return Constants.EvelynSystem;

                case ProjectCommands.AddToggle.Command c:
                    return c.ProjectId;
                case ProjectCommands.DeleteToggle.Command c:
                    return c.ProjectId;
                case ProjectCommands.AddEnvironment.Command c:
                    return c.ProjectId;
                case ProjectCommands.DeleteEnvironment.Command c:
                    return c.ProjectId;
                case ProjectCommands.ChangeToggleState.Command c:
                    return c.ProjectId;
                case ProjectCommands.DeleteProject.Command c:
                    return c.ProjectId;

                default:
                    throw new InvalidEnumArgumentException("Unrecognised command in test specs. Maybe you need to add it in CommandHandlerSpecs.ExtractAggregateId()");
            }
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
