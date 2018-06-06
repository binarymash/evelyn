namespace Evelyn.Core.Tests.WriteModel.Project.AddToggle
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Project.Commands.AddToggle;
    using Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class CommandSpecs : ProjectCommandHandlerSpecs<Handler, Command>
    {
        private Guid _projectId;

        private string _newToggleName;
        private string _newToggleKey;

        private string _environment1Key;
        private string _environment2Key;

        private string _existingToggleName;
        private string _existingToggleKey;

        private int _projectVersion = -1;

        [Fact]
        public void ProjectHasBeenDeleted()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveDeletedTheProject())
                .When(_ => WhenWeAddAToggle())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAProjectDeletedExceptionIsThrownFor(_projectId))
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void ToggleAlreadyExistsWithSameKey()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeAddAnotherToggleWithTheSameKey())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenADuplicateToggleKeyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void ToggleAlreadyExistsWithSameName()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeAddAnotherToggleWithTheSameName())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenADuplicateToggleNameExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void StaleProjectVersion()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenTheProjectVersionForOurNextCommandIsStale())
                .When(_ => WhenWeAddAToggle())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void FutureProjectVersion()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenTheProjectVersionForOurNextCommandIsInTheFuture())
                .When(_ => WhenWeAddAToggle())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void ToggleDoesntExistAndThereAreNoEnvironments()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .When(_ => WhenWeAddAToggle())

                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenAToggleAddedEventIsPublished())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(5))

                .And(_ => ThenTheAggregateRootHasHadAToggleAdded())
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(1))
                .And(_ => ThenTheAggregateRootScopedVersionHasBeenIncreasedBy(1))
                .BDDfy();
        }

        [Fact]
        public void ToggleDoesntExistAndThereAreMultipleEnvironments()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedTwoEnvironments())
                .When(_ => WhenWeAddAToggle())

                .Then(_ => ThenThreeEventsArePublished())

                .And(_ => ThenAToggleAddedEventIsPublished())
                .And(_ => ThenAnToggleStateAddedEventsIsPublishedForEnvironment1())
                .And(_ => ThenAnToggleStateAddedEventsIsPublishedForEnvironment2())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(13))

                .And(_ => ThenTheAggregateRootHasHadAToggleAdded())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(3))
                .And(_ => ThenTheAggregateRootScopedVersionHasBeenIncreasedBy(1))
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())

                .And(_ => ThenTheFirstEnvironmentStateHasANewToggleState())
                .And(_ => ThenTheFirstEnvironmentStateLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheFirstEnvironmentStateLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheFirstEnvironmentScopedVersionHasBeenIncreasedBy(1))

                .And(_ => ThenTheSecondEnvironmentStateHasANewToggleState())
                .And(_ => ThenTheSecondEnvironmentStateLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheSecondEnvironmentStateLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheSecondEnvironmentScopedVersionHasBeenIncreasedBy(1))
                .BDDfy();
        }

        protected override Handler BuildHandler()
        {
            return new Handler(Session);
        }

        private void GivenWeHaveCreatedAProject()
        {
            _projectId = DataFixture.Create<Guid>();

            GivenWeHaveCreatedAProjectWith(_projectId);
            _projectVersion++;
        }

        private void GivenWeHaveDeletedTheProject()
        {
            HistoricalEvents.Add(new ProjectDeleted(UserId, _projectId, DateTime.UtcNow) { Version = HistoricalEvents.Count });
            _projectVersion++;
        }

        private void GivenWeHaveAddedTwoEnvironments()
        {
            _environment1Key = DataFixture.Create<string>();
            _environment2Key = DataFixture.Create<string>();

            GivenWeHaveAddedAnEnvironmentWith(_projectId, _environment1Key);
            GivenWeHaveAddedAnEnvironmentStateWith(_projectId, _environment1Key);

            GivenWeHaveAddedAnEnvironmentWith(_projectId, _environment2Key);
            GivenWeHaveAddedAnEnvironmentStateWith(_projectId, _environment2Key);
            _projectVersion += 2;
        }

        private void GivenWeHaveAddedAToggle()
        {
            _existingToggleKey = DataFixture.Create<string>();
            _existingToggleName = DataFixture.Create<string>();

            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _existingToggleKey, _existingToggleName, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
            _projectVersion++;
        }

        private void GivenTheProjectVersionForOurNextCommandIsStale()
        {
            _projectVersion--;
        }

        private void GivenTheProjectVersionForOurNextCommandIsInTheFuture()
        {
            _projectVersion++;
        }

        private void WhenWeAddAToggle()
        {
            _newToggleKey = DataFixture.Create<string>();
            _newToggleName = DataFixture.Create<string>();

            var command = new Command(UserId, _projectId, _newToggleKey, _newToggleName, _projectVersion);
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameKey()
        {
            _newToggleKey = _existingToggleKey;
            _newToggleName = DataFixture.Create<string>();

            var command = new Command(UserId, _projectId, _newToggleKey, _newToggleName, _projectVersion);
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameName()
        {
            _newToggleKey = DataFixture.Create<string>();
            _newToggleName = _existingToggleName;

            var command = new Command(UserId, _projectId, _newToggleKey, _newToggleName, _projectVersion);
            WhenWeHandle(command);
        }

        private void ThenAToggleAddedEventIsPublished()
        {
            var ev = (ToggleAdded)PublishedEvents.First(e => e.GetType() == typeof(ToggleAdded));

            ev.UserId.Should().Be(UserId);
            ev.Name.Should().Be(_newToggleName);
            ev.Key.Should().Be(_newToggleKey);
        }

        private void ThenAnToggleStateAddedEventsIsPublishedForEnvironment1()
        {
            ThenAnToggleStateAddedEventsIsPublishedForEnvironment(_environment1Key);
        }

        private void ThenAnToggleStateAddedEventsIsPublishedForEnvironment2()
        {
            ThenAnToggleStateAddedEventsIsPublishedForEnvironment(_environment2Key);
        }

        private void ThenAnToggleStateAddedEventsIsPublishedForEnvironment(string environmentKey)
        {
            var ev = (ToggleStateAdded)PublishedEvents
                .First(e =>
                    e.GetType() == typeof(ToggleStateAdded) &&
                    ((ToggleStateAdded)e).EnvironmentKey == environmentKey);

            ev.ToggleKey.Should().Be(_newToggleKey);
            ev.Value.Should().Be(NewAggregate.Toggles.First(t => t.Key == _newToggleKey).DefaultValue);
            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            ev.UserId.Should().Be(UserId);
        }

        private void ThenADuplicateToggleKeyExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already a toggle with the key {_newToggleKey}");
        }

        private void ThenADuplicateToggleNameExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already a toggle with the name {_newToggleName}");
        }

        private void ThenTheAggregateRootHasHadAToggleAdded()
        {
            var toggle = NewAggregate.Toggles.First(e => e.Key == _newToggleKey);

            toggle.ScopedVersion.Should().Be(0);

            toggle.Name.Should().Be(_newToggleName);

            toggle.Created.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            toggle.CreatedBy.Should().Be(UserId);

            toggle.LastModified.Should().Be(toggle.Created);
            toggle.LastModifiedBy.Should().Be(toggle.CreatedBy);
        }

        private void ThenTheFirstEnvironmentStateHasANewToggleState()
        {
            ThenTheEnvironmentStateHasANewToggleState(_environment1Key);
        }

        private void ThenTheSecondEnvironmentStateHasANewToggleState()
        {
            ThenTheEnvironmentStateHasANewToggleState(_environment2Key);
        }

        private void ThenTheEnvironmentStateHasANewToggleState(string environmentKey)
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);
            var toggleState = environmentState.ToggleStates.First(e => e.Key == _newToggleKey);

            toggleState.Value.Should().Be(NewAggregate.Toggles.First(t => t.Key == _newToggleKey).DefaultValue);

            toggleState.Created.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            toggleState.CreatedBy.Should().Be(UserId);

            toggleState.LastModified.Should().Be(toggleState.Created);
            toggleState.LastModifiedBy.Should().Be(toggleState.CreatedBy);
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
