namespace Evelyn.Core.Tests.WriteModel.Project.DeleteEnvironment
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Project.Commands.DeleteEnvironment;
    using Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class CommandSpecs : ProjectCommandHandlerSpecs<Handler, Command>
    {
        private Guid _projectId;

        private string _environment1Key;
        private string _environment2Key;

        private int _environment1Version = -1;
        private int _environment2Version = -1;

        private string _environmentToDeleteKey;
        private int _environmentToDeleteVersion;

        public CommandSpecs()
        {
            IgnoreCollectionOrderDuringComparison = true;
        }

        [Fact]
        public void ProjectHasBeenDeleted()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .And(_ => GivenWeHaveAddedAnotherEnvironment())
                .And(_ => GivenWeHaveAddedTwoEnvironments())
                .And(_ => GivenWeWillBeDeletingTheFirstEnvironment())
                .And(_ => GivenWeHaveDeletedTheProject())

                .When(_ => WhenWeDeleteTheEnvironment())

                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAProjectDeletedExceptionIsThrownFor(_projectId))
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentDoesNotExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenOurEnvironmentDoesNotExist())
                .When(_ => WhenWeDeleteTheEnvironment())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAnUnknownEnvironmentExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void StaleExpectedEnvironmentVersion()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedTwoEnvironments())
                .And(_ => GivenWeWillBeDeletingTheFirstEnvironment())
                .And(_ => GivenTheExpectedEnvironmentVersionForOurNextCommandIsOffsetBy(-1))
                .When(_ => WhenWeDeleteTheEnvironment())

                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void EnvironmentExistsAndSoDoOtherEnvironments(int environmentVersionOffset)
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedTwoEnvironments())
                .And(_ => GivenWeWillBeDeletingTheFirstEnvironment())
                .And(_ => GivenTheExpectedEnvironmentVersionForOurNextCommandIsOffsetBy(environmentVersionOffset))

                .When(_ => WhenWeDeleteTheEnvironment())

                .Then(_ => ThenTwoEventsArePublished())

                .And(_ => ThenAnEnvironmentDeletedEventIsPublished())
                .And(_ => ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment1())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(8))

                .And(_ => ThenTheAggregateRootHasOneFewerEnvironments())
                .And(_ => ThenTheAggregateRootHasHadTheCorrectEnvironmentRemoved())

                .And(_ => ThenTheAggregateRootHasOneFewerEnvironmentStates())
                .And(_ => ThenTheAggregateRootHasHadTheCorrectEnvironmentStateRemoved())

                .And(_ => ThenTheAggregateRootLastModifiedVersionIs(NewAggregate.Version - 1))
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(2))

                .BDDfy();
        }

        protected override Handler BuildHandler()
        {
            return new Handler(Logger, Session);
        }

        private void GivenWeHaveCreatedAProject()
        {
            _projectId = DataFixture.Create<Guid>();
            GivenWeHaveCreatedAProjectWith(_projectId);
        }

        private void GivenWeHaveAddedTwoEnvironments()
        {
            _environment1Key = DataFixture.Create<string>();
            _environment2Key = DataFixture.Create<string>();

            GivenWeHaveAddedAnEnvironmentWith(_projectId, _environment1Key);
            _environment1Version = HistoricalEvents.Count - 1;
            GivenWeHaveAddedAnEnvironmentStateWith(_projectId, _environment1Key);

            GivenWeHaveAddedAnEnvironmentWith(_projectId, _environment2Key);
            _environment2Version = HistoricalEvents.Count - 1;
            GivenWeHaveAddedAnEnvironmentStateWith(_projectId, _environment2Key);
        }

        private void GivenOurEnvironmentDoesNotExist()
        {
            _environmentToDeleteKey = DataFixture.Create<string>();
            _environmentToDeleteVersion = DataFixture.Create<int>();
        }

        private void GivenWeWillBeDeletingTheFirstEnvironment()
        {
            _environmentToDeleteKey = _environment1Key;
            _environmentToDeleteVersion = _environment1Version;
        }

        private void GivenWeHaveDeletedTheProject()
        {
            HistoricalEvents.Add(new ProjectDeleted(UserId, _projectId, DateTime.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void GivenWeHaveAddedAnEnvironment()
        {
            _environment1Key = DataFixture.Create<string>();
            HistoricalEvents.Add(new EnvironmentAdded(UserId, _projectId, _environment1Key, DataFixture.Create<string>(), DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
            _environment1Version = HistoricalEvents.Count - 1;
        }

        private void GivenWeHaveAddedAnotherEnvironment()
        {
            _environment2Key = DataFixture.Create<string>();
            HistoricalEvents.Add(new EnvironmentAdded(UserId, _projectId, _environment2Key, DataFixture.Create<string>(), DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
            _environment2Version = HistoricalEvents.Count - 1;
        }

        private void GivenTheExpectedEnvironmentVersionForOurNextCommandIsOffsetBy(int environmentVersionOffset)
        {
            _environmentToDeleteVersion += environmentVersionOffset;
        }

        private void WhenWeDeleteTheEnvironment()
        {
            UserId = DataFixture.Create<string>();
            var command = new Command(UserId, _projectId, _environmentToDeleteKey, _environmentToDeleteVersion);
            WhenWeHandle(command);
        }

        private void ThenAnEnvironmentDeletedEventIsPublished()
        {
            var ev = (EnvironmentDeleted)PublishedEvents.First(e => e.GetType() == typeof(EnvironmentDeleted));

            ev.UserId.Should().Be(UserId);
            ev.Key.Should().Be(_environmentToDeleteKey);
            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment1()
        {
            ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment(_environment1Key);
        }

        private void ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment2()
        {
            ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment(_environment2Key);
        }

        private void ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment(string environmentKey)
        {
            var ev = (EnvironmentStateDeleted)PublishedEvents
                .First(e =>
                    e.GetType() == typeof(EnvironmentStateDeleted) &&
                    ((EnvironmentStateDeleted)e).EnvironmentKey == environmentKey);

            ev.EnvironmentKey.Should().Be(_environmentToDeleteKey);
            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            ev.UserId.Should().Be(UserId);
        }

        private void ThenAnUnknownEnvironmentExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is no environment with the key {_environmentToDeleteKey}");
        }

        private void ThenTheAggregateRootHasOneFewerEnvironments()
        {
            NewAggregate.Environments.Count().Should().Be(OriginalAggregate.Environments.Count() - 1);
        }

        private void ThenTheAggregateRootHasHadTheCorrectEnvironmentRemoved()
        {
            NewAggregate.Environments.Any(t => t.Key == _environmentToDeleteKey).Should().BeFalse();
        }

        private void ThenTheAggregateRootHasOneFewerEnvironmentStates()
        {
            NewAggregate.EnvironmentStates.Count().Should().Be(OriginalAggregate.EnvironmentStates.Count() - 1);
        }

        private void ThenTheAggregateRootHasHadTheCorrectEnvironmentStateRemoved()
        {
            NewAggregate.EnvironmentStates.Any(t => t.EnvironmentKey == _environmentToDeleteKey).Should().BeFalse();
        }
    }
}
