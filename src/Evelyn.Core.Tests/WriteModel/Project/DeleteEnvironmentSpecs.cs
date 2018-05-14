namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Project.Commands;
    using Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class DeleteEnvironmentSpecs : ProjectCommandHandlerSpecs<DeleteEnvironment>
    {
        private Guid _projectId;

        private string _environment1Key;
        private string _environment2Key;

        private int _environment1Version = -1;
        private int _environment2Version = -1;

        private string _environmentToDeleteKey;
        private int _environmentToDeleteVersion;

        public DeleteEnvironmentSpecs()
        {
            IgnoreCollectionOrderDuringComparison = true;
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
        public void StaleToggleVersion()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .And(_ => GivenWeHaveAddedAnotherEnvironment())
                .And(_ => GivenWeWillBeDeletingTheFirstEnvironment())
                .And(_ => GivenTheEnvironmentVersionForOurNextCommandIsStale())
                .When(_ => WhenWeDeleteTheEnvironment())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void FutureProjectVersion()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .And(_ => GivenWeHaveAddedAnotherEnvironment())
                .And(_ => GivenWeWillBeDeletingTheFirstEnvironment())
                .And(_ => GivenTheEnvironmentVersionForOurNextCommandIsInTheFuture())
                .When(_ => WhenWeDeleteTheEnvironment())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentExistsAndSoDoOtherEnvironments()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .And(_ => GivenWeHaveAddedAnotherEnvironment())
                .And(_ => GivenWeHaveAddedTwoEnvironments())
                .And(_ => GivenWeWillBeDeletingTheFirstEnvironment())

                .When(_ => WhenWeDeleteTheEnvironment())

                .Then(_ => ThenTwoEventsArePublished())

                .And(_ => ThenAnEnvironmentDeletedEventIsPublished())
                .And(_ => ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment1())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(8))

                .And(_ => ThenTheAggregateRootHasOneFewerEnvironments())
                .And(_ => ThenTheAggregateRootHasHadTheCorrectEnvironmentRemoved())

                .And(_ => ThenTheAggregateRootHasOneFewerEnvironmentStates())
                .And(_ => ThenTheAggregateRootHasHadTheCorrectEnvironmentStateRemoved())

                .And(_ => ThenTheAggregateRootScopedVersionHasBeenIncreasedBy(1))
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(2))

                .BDDfy();
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
            GivenWeHaveAddedAnEnvironmentStateWith(_projectId, _environment1Key);

            GivenWeHaveAddedAnEnvironmentWith(_projectId, _environment2Key);
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

        private void GivenWeHaveAddedAnEnvironment()
        {
            _environment1Key = DataFixture.Create<string>();
            HistoricalEvents.Add(new EnvironmentAdded(UserId, _projectId, _environment1Key, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
            _environment1Version++;
        }

        private void GivenWeHaveAddedAnotherEnvironment()
        {
            _environment2Key = DataFixture.Create<string>();
            HistoricalEvents.Add(new EnvironmentAdded(UserId, _projectId, _environment2Key, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
            _environment2Version++;
        }

        private void GivenTheEnvironmentVersionForOurNextCommandIsStale()
        {
            _environmentToDeleteVersion--;
        }

        private void GivenTheEnvironmentVersionForOurNextCommandIsInTheFuture()
        {
            _environmentToDeleteVersion++;
        }

        private void WhenWeDeleteTheEnvironment()
        {
            UserId = DataFixture.Create<string>();
            var command = new DeleteEnvironment(UserId, _projectId, _environmentToDeleteKey, _environmentToDeleteVersion);
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
