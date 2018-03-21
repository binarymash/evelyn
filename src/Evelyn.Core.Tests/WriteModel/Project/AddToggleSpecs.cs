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

    public class AddToggleSpecs : ProjectCommandHandlerSpecs<AddToggle>
    {
        private Guid _projectId;

        private string _newToggleName;
        private string _newToggleKey;

        private string _environment1Key;
        private string _environment2Key;

        private string _existingToggleName;
        private string _existingToggleKey;

        [Fact]
        public void ToggleDoesntExistAndThereAreNoEnvironments()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .When(_ => WhenWeAddAToggle())

                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenAToggleAddedEventIsPublished())

                .And(_ => ThenThereAreFourChangesOnTheAggregate())

                .And(_ => ThenTheAggregateRootHasHadAToggleAdded())
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedByOne())
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

                .And(_ => ThenThereAreTwelveChangesOnTheAggregate())

                .And(_ => ThenTheAggregateRootHasHadAToggleAdded())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedByThree())
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())

                .And(_ => ThenTheFirstEnvironmentStateHasANewToggleState())
                .And(_ => ThenTheFirstEnvironmentStateVersionHasBeenIncremented())
                .And(_ => ThenTheFirstEnvironmentStateLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheFirstEnvironmentStateLastModifiedByHasBeenUpdated())

                .And(_ => ThenTheSecondEnvironmentStateHasANewToggleState())
                .And(_ => ThenTheSecondEnvironmentStateVersionHasBeenIncremented())
                .And(_ => ThenTheSecondEnvironmentStateLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheSecondEnvironmentStateLastModifiedByHasBeenUpdated())
                .BDDfy();
        }

        [Fact]
        public void ToggleAlreadyExistWithSameKey()
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
        public void ToggleAlreadyExistWithSameName()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeAddAnotherToggleWithTheSameName())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateToggleNameExceptionIsThrown())
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

        private void GivenWeHaveAddedAToggle()
        {
            _existingToggleKey = DataFixture.Create<string>();
            _existingToggleName = DataFixture.Create<string>();

            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _existingToggleKey, _existingToggleName, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void WhenWeAddAToggle()
        {
            _newToggleKey = DataFixture.Create<string>();
            _newToggleName = DataFixture.Create<string>();

            var command = new AddToggle(UserId, _projectId, _newToggleKey, _newToggleName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameKey()
        {
            _newToggleKey = _existingToggleKey;
            _newToggleName = DataFixture.Create<string>();

            var command = new AddToggle(UserId, _projectId, _newToggleKey, _newToggleName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameName()
        {
            _newToggleKey = DataFixture.Create<string>();
            _newToggleName = _existingToggleName;

            var command = new AddToggle(UserId, _projectId, _newToggleKey, _newToggleName) { ExpectedVersion = HistoricalEvents.Count - 1 };
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
            ev.OccurredAt.Should().BeOnOrAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
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

            toggle.Name.Should().Be(_newToggleName);

            toggle.Created.Should().BeOnOrAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
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

            toggleState.Created.Should().BeOnOrAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            toggleState.CreatedBy.Should().Be(UserId);

            toggleState.LastModified.Should().Be(toggleState.Created);
            toggleState.LastModifiedBy.Should().Be(toggleState.CreatedBy);
        }

        private void ThenTheFirstEnvironmentStateVersionHasBeenIncremented()
        {
            ThenTheEnvironmentStateVersionHasBeenIncremented(_environment1Key);
        }

        private void ThenTheSecondEnvironmentStateVersionHasBeenIncremented()
        {
            ThenTheEnvironmentStateVersionHasBeenIncremented(_environment2Key);
        }

        private void ThenTheEnvironmentStateVersionHasBeenIncremented(string environmentKey)
        {
            var newEnvironmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);
            var oldEnvironmentState = OriginalAggregate.EnvironmentStates.First(es => es.EnvironmentKey == environmentKey);

            newEnvironmentState.Version.Should().Be(oldEnvironmentState.Version + 1);
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
            environmentState.LastModified.Should().BeOnOrAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
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
    }
}
