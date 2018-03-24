namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Project.Commands;
    using Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class DeleteToggleSpecs : ProjectCommandHandlerSpecs<DeleteToggle>
    {
        private Guid _projectId;

        private string _environment1Key;
        private string _environment2Key;

        private string _toggle1Key;
        private int _toggle1Version = -1;

        private string _toggle2Key;
        private int _toggle2Version = -1;

        private string _toggleToDeleteKey;
        private int _toggleToDeleteVersion;

        public DeleteToggleSpecs()
        {
            IgnoreCollectionOrderDuringComparison = true;
        }

        [Fact]
        public void ToggleDoesNotExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenOurToggleDoesNotExist())
                .When(_ => WhenWeDeleteTheToggle())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAnUnknownToggleExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void StaleToggleVersion()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenWeHaveAddedAnotherToggle())
                .And(_ => GivenWeWillBeDeletingTheFirstToggle())
                .And(_ => GivenTheToggleVersionForOurNextCommandIsStale())
                .When(_ => WhenWeDeleteTheToggle())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void FutureProjectVersion()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenWeHaveAddedAnotherToggle())
                .And(_ => GivenWeWillBeDeletingTheFirstToggle())
                .And(_ => GivenTheToggleVersionForOurNextCommandIsInTheFuture())
                .When(_ => WhenWeDeleteTheToggle())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void ToggleExistsAndThereAreNoEnvironments()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenWeHaveAddedAnotherToggle())
                .And(_ => GivenWeWillBeDeletingTheFirstToggle())

                .When(_ => WhenWeDeleteTheToggle())

                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenAToggleDeletedEventIsPublished())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(6))

                .And(_ => ThenTheAggregateRootHasOneFewerToggles())
                .And(_ => ThenTheAggregateRootHasHadTheCorrectToggleRemoved())
                .And(_ => ThenTheAggregateRootScopedVersionHasBeenIncreasedBy(1))
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(1))
                .BDDfy();
        }

        [Fact]
        public void ToggleExistsAndThereAreMultipleEnvironments()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenWeHaveAddedAnotherToggle())
                .And(_ => GivenWeHaveAddedTwoEnvironments())
                .And(_ => GivenWeWillBeDeletingTheFirstToggle())

                .When(_ => WhenWeDeleteTheToggle())

                .Then(_ => ThenThreeEventsArePublished())

                .And(_ => ThenAToggleDeletedEventIsPublished())
                .And(_ => ThenAnToggleStateDeletedEventsIsPublishedForEnvironment1())
                .And(_ => ThenAnToggleStateDeletedEventsIsPublishedForEnvironment2())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(16))

                .And(_ => ThenTheAggregateRootHasOneFewerToggles())
                .And(_ => ThenTheAggregateRootHasHadTheCorrectToggleRemoved())

                .And(_ => ThenTheFirstEnvironmentStateHasOneFewerToggleStates())
                .And(_ => ThenTheFirstEnvironmentStateHasHadTheCorrectToggleStateDeleted())
                .And(_ => ThenTheFirstEnvironmentScopedVersionHasBeenIncreasedBy(1))
                .And(_ => ThenTheFirstEnvironmentStateLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheFirstEnvironmentStateLastModifiedByHasBeenUpdated())

                .And(_ => ThenTheSecondEnvironmentStateHasOneFewerToggleStates())
                .And(_ => ThenTheSecondEnvironmentStateHasHadTheCorrectToggleStateDeleted())
                .And(_ => ThenTheSecondEnvironmentScopedVersionHasBeenIncreasedBy(1))
                .And(_ => ThenTheSecondEnvironmentStateLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheSecondEnvironmentStateLastModifiedByHasBeenUpdated())

                .And(_ => ThenTheAggregateRootScopedVersionHasBeenIncreasedBy(1))
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(3))

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
            GivenWeHaveAddedAnEnvironmentStateWith(
                _projectId,
                _environment1Key,
                new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>(_toggle1Key, default), new KeyValuePair<string, string>(_toggle2Key, default) });

            GivenWeHaveAddedAnEnvironmentWith(_projectId, _environment2Key);
            GivenWeHaveAddedAnEnvironmentStateWith(
                _projectId,
                _environment2Key,
                new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(_toggle1Key, default), new KeyValuePair<string, string>(_toggle2Key, default) });
        }

        private void GivenOurToggleDoesNotExist()
        {
            _toggleToDeleteKey = DataFixture.Create<string>();
            _toggleToDeleteVersion = DataFixture.Create<int>();
        }

        private void GivenWeWillBeDeletingTheFirstToggle()
        {
            _toggleToDeleteKey = _toggle1Key;
            _toggleToDeleteVersion = _toggle1Version;
        }

        private void GivenWeHaveAddedAToggle()
        {
            _toggle1Key = DataFixture.Create<string>();
            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _toggle1Key, DataFixture.Create<string>(), DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });

            _toggle1Version++;
        }

        private void GivenWeHaveAddedAnotherToggle()
        {
            _toggle2Key = DataFixture.Create<string>();
            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _toggle2Key, DataFixture.Create<string>(), DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
            _toggle2Version++;
        }

        private void GivenTheToggleVersionForOurNextCommandIsStale()
        {
            _toggleToDeleteVersion--;
        }

        private void GivenTheToggleVersionForOurNextCommandIsInTheFuture()
        {
            _toggleToDeleteVersion++;
        }

        private void WhenWeDeleteTheToggle()
        {
            UserId = DataFixture.Create<string>();
            var command = new DeleteToggle(UserId, _projectId, _toggleToDeleteKey, _toggleToDeleteVersion);
            WhenWeHandle(command);
        }

        private void ThenAToggleDeletedEventIsPublished()
        {
            var ev = (ToggleDeleted)PublishedEvents.First(e => e.GetType() == typeof(ToggleDeleted));

            ev.UserId.Should().Be(UserId);
            ev.Key.Should().Be(_toggleToDeleteKey);
            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenAnToggleStateDeletedEventsIsPublishedForEnvironment1()
        {
            ThenAnToggleStateDeletedEventsIsPublishedForEnvironment(_environment1Key);
        }

        private void ThenAnToggleStateDeletedEventsIsPublishedForEnvironment2()
        {
            ThenAnToggleStateDeletedEventsIsPublishedForEnvironment(_environment2Key);
        }

        private void ThenAnToggleStateDeletedEventsIsPublishedForEnvironment(string environmentKey)
        {
            var ev = (ToggleStateDeleted)PublishedEvents
                .First(e =>
                    e.GetType() == typeof(ToggleStateDeleted) &&
                    ((ToggleStateDeleted)e).EnvironmentKey == environmentKey);

            ev.ToggleKey.Should().Be(_toggleToDeleteKey);
            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            ev.UserId.Should().Be(UserId);
        }

        private void ThenAnUnknownToggleExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is no toggle with the key {_toggleToDeleteKey}");
        }

        private void ThenTheAggregateRootHasOneFewerToggles()
        {
            NewAggregate.Toggles.Count().Should().Be(OriginalAggregate.Toggles.Count() - 1);
        }

        private void ThenTheAggregateRootHasHadTheCorrectToggleRemoved()
        {
            NewAggregate.Toggles.Any(t => t.Key == _toggleToDeleteKey).Should().BeFalse();
        }

        private void ThenTheFirstEnvironmentStateHasOneFewerToggleStates()
        {
            ThenTheEnvironmentStateHasOneFewerToggleStated(_environment1Key);
        }

        private void ThenTheSecondEnvironmentStateHasOneFewerToggleStates()
        {
            ThenTheEnvironmentStateHasOneFewerToggleStated(_environment2Key);
        }

        private void ThenTheEnvironmentStateHasOneFewerToggleStated(string environmentKey)
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);
            var oldEnvironmentState = OriginalAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);
            environmentState.ToggleStates.Count().Should().Be(oldEnvironmentState.ToggleStates.Count() - 1);
        }

        private void ThenTheFirstEnvironmentStateHasHadTheCorrectToggleStateDeleted()
        {
            ThenTheEnvironmentStateHasHadTheToggleStateDeleted(_environment1Key);
        }

        private void ThenTheSecondEnvironmentStateHasHadTheCorrectToggleStateDeleted()
        {
            ThenTheEnvironmentStateHasHadTheToggleStateDeleted(_environment2Key);
        }

        private void ThenTheEnvironmentStateHasHadTheToggleStateDeleted(string environmentKey)
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);
            environmentState.ToggleStates.Any(ts => ts.Key == _toggleToDeleteKey).Should().BeFalse();
        }

        private void ThenTheFirstEnvironmentStateLastModifiedTimeHasBeenUpdated()
        {
            ThenTheEnvironmentStateLastModifiedTimeHasBeenUpdated(_environment1Key);
        }

        private void ThenTheSecondEnvironmentStateLastModifiedTimeHasBeenUpdated()
        {
            ThenTheEnvironmentStateLastModifiedTimeHasBeenUpdated(_environment2Key);
        }

        private void ThenTheEnvironmentStateLastModifiedTimeHasBeenUpdated(string environmentKey)
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);
            environmentState.LastModified.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenTheFirstEnvironmentStateLastModifiedByHasBeenUpdated()
        {
            ThenTheEnvironmentStateLastModifiedByHasBeenUpdated(_environment1Key);
        }

        private void ThenTheSecondEnvironmentStateLastModifiedByHasBeenUpdated()
        {
            ThenTheEnvironmentStateLastModifiedByHasBeenUpdated(_environment2Key);
        }

        private void ThenTheEnvironmentStateLastModifiedByHasBeenUpdated(string environmentKey)
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);
            environmentState.LastModifiedBy.Should().Be(UserId);
        }

        private void ThenTheFirstEnvironmentScopedVersionHasBeenIncreasedBy(int increment)
        {
            ThenTheEnvironmentScopedVersionHasBeenIncreasedBy(_environment1Key, increment);
        }

        private void ThenTheSecondEnvironmentScopedVersionHasBeenIncreasedBy(int increment)
        {
            ThenTheEnvironmentScopedVersionHasBeenIncreasedBy(_environment2Key, increment);
        }

        private void ThenTheEnvironmentScopedVersionHasBeenIncreasedBy(string environmentKey, int increment)
        {
            var newEnvironmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);
            var oldEnvironmentState = OriginalAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);

            newEnvironmentState.ScopedVersion.Should().Be(oldEnvironmentState.ScopedVersion + increment);
        }
    }
}
