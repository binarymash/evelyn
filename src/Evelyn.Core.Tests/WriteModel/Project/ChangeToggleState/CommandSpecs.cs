namespace Evelyn.Core.Tests.WriteModel.Project.ChangeToggleState
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Project.Commands.ChangeToggleState;
    using Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class CommandSpecs : ProjectCommandHandlerSpecs<Handler, Command>
    {
        private Guid _projectId;
        private string _environmentKey;
        private string _toggleKey;
        private string _toggleName;
        private string _toggleValue;
        private string _newToggleValue;
        private int _toggleStateVersion = -1;

        [Fact]
        public void ProjectHasBeenDeleted()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenWeHaveDeletedTheProject())
                .When(_ => WhenWeChangeTheToggleState())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAProjectDeletedExceptionIsThrownFor(_projectId))
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .When(_ => WhenWeChangeTheValueOfAToggleOnAnEnvironmentThatDoesntExist())
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
        public void StaleToggleStateVersion()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenWeHaveChangedTheToggleState())
                .And(_ => GivenTheToggleStateVersionForOurNextCommandIsStale())
                .When(_ => WhenWeChangeTheToggleState())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Nominal(int toggleStateVersionOffset)
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenTheToggleStateVersionForOurNextCommandIsInTheFutureBy(toggleStateVersionOffset))
                .When(_ => WhenWeChangeTheToggleState())

                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsToggledValueChanged())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(8))

                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(1))

                .And(_ => ThenTheEnvironmentStateLastModifiedVersionIs(NewAggregate.Version))
                .And(_ => ThenTheEnvironmentStateLastModifiedHasBeenUpdated())
                .And(_ => ThenTheEnvironmentStateLastModifiedByHasBeenUpdated())

                .And(_ => ThenTheToggleStateValueHasBeenUpdated())
                .And(_ => ThenTheToggleStateLastModifiedVersionIs(NewAggregate.Version))
                .And(_ => ThenTheToggleStateLastModifiedHasBeenUpdated())
                .And(_ => ThenTheToggleStateLastModifiedByHasBeenUpdated())

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

        private void GivenWeHaveDeletedTheProject()
        {
            HistoricalEvents.Add(new ProjectDeleted(UserId, _projectId, DateTime.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void GivenWeHaveCreatedAnEnvironment()
        {
            _environmentKey = TestUtilities.CreateKey(30);

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

            _toggleStateVersion = HistoricalEvents.Count - 1;
        }

        private void GivenWeHaveChangedTheToggleState()
        {
            UserId = DataFixture.Create<string>();
            _newToggleValue = DataFixture.Create<bool>().ToString();

            HistoricalEvents.Add(new ToggleStateChanged(UserId, _projectId, _environmentKey, _toggleKey, _newToggleValue, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
            _toggleStateVersion = HistoricalEvents.Count - 1;
        }

        private void GivenTheToggleStateVersionForOurNextCommandIsStale()
        {
            _toggleStateVersion--;
        }

        private void GivenTheToggleStateVersionForOurNextCommandIsInTheFutureBy(int toggleStateVersionOffset)
        {
            _toggleStateVersion += toggleStateVersionOffset;
        }

        private void WhenWeChangeTheValueOfAToggleOnAnEnvironmentThatDoesntExist()
        {
            UserId = DataFixture.Create<string>();
            _environmentKey = TestUtilities.CreateKey(30);
            _toggleKey = TestUtilities.CreateKey(30);
            _newToggleValue = DataFixture.Create<bool>().ToString();
            _toggleStateVersion = 0;

            var command = new Command(UserId, _projectId, _environmentKey, _toggleKey, _newToggleValue, _toggleStateVersion);
            WhenWeHandle(command);
        }

        private void WhenWeChangeTheValueOfAToggleThatDoesntExist()
        {
            UserId = DataFixture.Create<string>();
            _toggleKey = TestUtilities.CreateKey(30);
            _newToggleValue = DataFixture.Create<bool>().ToString();
            _toggleStateVersion = 0;

            var command = new Command(UserId, _projectId, _environmentKey, _toggleKey, _newToggleValue, _toggleStateVersion);
            WhenWeHandle(command);
        }

        private void WhenWeChangeTheToggleStateToAnInvalidValue()
        {
            UserId = DataFixture.Create<string>();
            _newToggleValue = DataFixture.Create<string>();

            var command = new Command(UserId, _projectId, _environmentKey, _toggleKey, _newToggleValue, _toggleStateVersion);
            WhenWeHandle(command);
        }

        private void WhenWeChangeTheToggleState()
        {
            UserId = DataFixture.Create<string>();
            _newToggleValue = DataFixture.Create<bool>().ToString();

            var command = new Command(UserId, _projectId, _environmentKey, _toggleKey, _newToggleValue, _toggleStateVersion);
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
            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            ev.UserId.Should().Be(UserId);
        }

        private void ThenTheEnvironmentStateLastModifiedVersionIs(int expectedLastModifiedVersion)
        {
            var newEnvironmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            newEnvironmentState.LastModifiedVersion.Should().Be(expectedLastModifiedVersion);
        }

        private void ThenTheEnvironmentStateLastModifiedHasBeenUpdated()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            environmentState.LastModified.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
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

        private void ThenTheToggleStateLastModifiedVersionIs(int expectedLastModifiedVersion)
        {
            var newEnvironmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            var newToggleState = newEnvironmentState.ToggleStates.First(ts => ts.Key == _toggleKey);
            newToggleState.LastModifiedVersion.Should().Be(expectedLastModifiedVersion);
        }

        private void ThenTheToggleStateLastModifiedHasBeenUpdated()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            var toggleState = environmentState.ToggleStates.First(ts => ts.Key == _toggleKey);

            toggleState.LastModified.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenTheToggleStateLastModifiedByHasBeenUpdated()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _environmentKey);
            var toggleState = environmentState.ToggleStates.First(ts => ts.Key == _toggleKey);

            toggleState.LastModifiedBy.Should().Be(UserId);
        }
    }
}
