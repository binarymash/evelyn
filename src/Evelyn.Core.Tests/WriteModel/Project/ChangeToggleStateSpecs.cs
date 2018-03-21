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

    public class ChangeToggleStateSpecs : ProjectCommandHandlerSpecs<ChangeToggleState>
    {
        private Guid _projectId;
        private string _environmentKey;
        private string _toggleKey;
        private string _toggleName;
        private string _toggleValue;
        private string _newToggleValue;

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .When(_ => WhenWeChangeTheValueOfAToggleThatDoesntExist())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAnEnvironmentDoesNotExistExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void ToggleDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .When(_ => WhenWeChangeTheValueOfAToggleThatDoesntExist())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAToggleDoesNotExistExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void InvalidToggleState()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeChangeTheToggleStateToAnInvalidValue())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAnInvalidToggleStateExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void ValidToggleState()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeChangeTheToggleState())

                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsToggledValueChanged())

                .And(_ => ThenThereAreEightChangesOnTheAggregate())

                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedByOne())

                .And(_ => ThenTheEnvironmentStateVersionHasIncreasedByOne())
                .And(_ => ThenTheEnvironmentStateLastModifiedHasBeenUpdated())
                .And(_ => ThenTheEnvironmentStateLastModifiedByHasBeenUpdated())

                .And(_ => ThenTheToggleStateValueHasBeenUpdated())
                .And(_ => ThenTheToggleStateVersionHasIncreasedByOne())
                .And(_ => ThenTheToggleStateLastModifiedHasBeenUpdated())
                .And(_ => ThenTheToggleStateLastModifiedByHasBeenUpdated())

                .BDDfy();
        }

        private void GivenWeHaveCreatedAProject()
        {
            _projectId = DataFixture.Create<Guid>();

            GivenWeHaveCreatedAProjectWith(_projectId);
        }

        private void GivenWeHaveCreatedAnEnvironment()
        {
            _environmentKey = DataFixture.Create<string>();

            GivenWeHaveAddedAnEnvironmentWith(_projectId, _environmentKey);
            GivenWeHaveAddedAnEnvironmentStateWith(_projectId, _environmentKey);
        }

        private void GivenWeHaveAddedAToggle()
        {
            _toggleName = DataFixture.Create<string>();
            _toggleKey = DataFixture.Create<string>();
            _toggleValue = DataFixture.Create<bool>().ToString();

            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _toggleKey, _toggleName, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
            HistoricalEvents.Add(new ToggleStateAdded(UserId, _projectId, _environmentKey, _toggleKey, _toggleValue, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void WhenWeChangeTheValueOfAToggleThatDoesntExist()
        {
            UserId = DataFixture.Create<string>();
            _toggleKey = DataFixture.Create<string>();
            _newToggleValue = DataFixture.Create<bool>().ToString();

            var command = new ChangeToggleState(UserId, _projectId, _environmentKey, _toggleKey, _newToggleValue) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeChangeTheToggleStateToAnInvalidValue()
        {
            UserId = DataFixture.Create<string>();
            _newToggleValue = DataFixture.Create<string>();

            var command = new ChangeToggleState(UserId, _projectId, _environmentKey, _toggleKey, _newToggleValue) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeChangeTheToggleState()
        {
            UserId = DataFixture.Create<string>();
            _newToggleValue = DataFixture.Create<bool>().ToString();

            var command = new ChangeToggleState(UserId, _projectId, _environmentKey, _toggleKey, _newToggleValue) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void ThenAnEnvironmentDoesNotExistExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is no environment with the key {_environmentKey}");
        }

        private void ThenAToggleDoesNotExistExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is no toggle with the key {_toggleKey}");
        }

        private void ThenAnInvalidToggleStateExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage("Invalid toggle value");
        }

        private void ThenThePublishedEventIsToggledValueChanged()
        {
            var ev = (ToggleStateChanged)PublishedEvents.First();

            ev.Id.Should().Be(_projectId);
            ev.EnvironmentKey.Should().Be(_environmentKey);
            ev.ToggleKey.Should().Be(_toggleKey);
            ev.Value.Should().Be(_newToggleValue);
            ev.OccurredAt.Should().BeOnOrAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            ev.UserId.Should().Be(UserId);
        }

        private void ThenTheEnvironmentStateVersionHasIncreasedByOne()
        {
            var newEnvironmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            var oldEnvironmentState = OriginalAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);

            newEnvironmentState.Version.Should().Be(oldEnvironmentState.Version + 1);
        }

        private void ThenTheEnvironmentStateLastModifiedHasBeenUpdated()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            environmentState.LastModified.Should().BeOnOrAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenTheEnvironmentStateLastModifiedByHasBeenUpdated()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            environmentState.LastModifiedBy.Should().Be(UserId);
        }

        private void ThenTheToggleStateValueHasBeenUpdated()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            var toggleState = environmentState.ToggleStates.First(ts => ts.Key == _toggleKey);

            toggleState.Value.Should().Be(_newToggleValue);
        }

        private void ThenTheToggleStateVersionHasIncreasedByOne()
        {
            var newEnvironmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            var newToggleState = newEnvironmentState.ToggleStates.First(ts => ts.Key == _toggleKey);

            var oldEnvironmentState = OriginalAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            var oldToggleState = oldEnvironmentState.ToggleStates.First(ts => ts.Key == _toggleKey);

            newToggleState.Version.Should().Be(oldToggleState.Version + 1);
        }

        private void ThenTheToggleStateLastModifiedHasBeenUpdated()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            var toggleState = environmentState.ToggleStates.First(ts => ts.Key == _toggleKey);

            toggleState.LastModified.Should().BeOnOrAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenTheToggleStateLastModifiedByHasBeenUpdated()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            var toggleState = environmentState.ToggleStates.First(ts => ts.Key == _toggleKey);

            toggleState.LastModifiedBy.Should().Be(UserId);
        }
    }
}
