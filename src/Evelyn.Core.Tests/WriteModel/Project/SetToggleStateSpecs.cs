namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Commands;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class SetToggleStateSpecs : ProjectCommandHandlerSpecs<ChangeToggleState>
    {
        private Guid _projectId;

        private string _environmentKey;

        private Guid _toggleId;
        private string _toggleName;
        private string _toggleKey;
        private string _toggleState;

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .When(_ => WhenWeChangeTheValueOfAToggleThatDoesntExist())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAnEnvironmentDoesNotExistExceptionIsThrown())
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
                .BDDfy();
        }

        [Fact]
        public void ValidToggleState()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeChangeTheToggleState())
                .Then(_ => ThenThePublishedEventIsToggledValueChanged())
                .And(_ => ThenTheUserIdIsSaved())
                .And(_ => ThenTheProjectIdIsSaved())
                .And(_ => ThenTheEnvironmentKeyIsSaved())
                .And(_ => ThenTheToggleIdIsSaved())
                .And(_ => ThenTheToggleStateIsSaved())
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
        }

        private void GivenWeHaveAddedAToggle()
        {
            _toggleId = DataFixture.Create<Guid>();
            _toggleName = DataFixture.Create<string>();
            _toggleKey = DataFixture.Create<string>();

            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _toggleId, _toggleName, _toggleKey) { Version = HistoricalEvents.Count });
        }

        private void WhenWeChangeTheValueOfAToggleThatDoesntExist()
        {
            _toggleId = DataFixture.Create<Guid>();
            _toggleState = DataFixture.Create<bool>().ToString();

            var command = new ChangeToggleState(UserId, _projectId, _environmentKey, _toggleId, _toggleState) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeChangeTheToggleStateToAnInvalidValue()
        {
            _toggleState = DataFixture.Create<string>();

            var command = new ChangeToggleState(UserId, _projectId, _environmentKey, _toggleId, _toggleState) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeChangeTheToggleState()
        {
            _toggleState = DataFixture.Create<bool>().ToString();

            var command = new ChangeToggleState(UserId, _projectId, _environmentKey, _toggleId, _toggleState) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void ThenAnEnvironmentDoesNotExistExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is no environment with the key {_environmentKey}");
        }

        private void ThenAToggleDoesNotExistExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is no toggle with the ID {_toggleId}");
        }

        private void ThenAnInvalidToggleStateExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage("Invalid toggle value");
        }

        private void ThenThePublishedEventIsToggledValueChanged()
        {
            PublishedEvents.First().Should().BeOfType<ToggleStateChanged>();
        }

        private void ThenTheUserIdIsSaved()
        {
            ((ToggleStateChanged)PublishedEvents.First()).UserId.Should().Be(UserId);
        }

        private void ThenTheProjectIdIsSaved()
        {
            ((ToggleStateChanged)PublishedEvents.First()).Id.Should().Be(_projectId);
        }

        private void ThenTheEnvironmentKeyIsSaved()
        {
            ((ToggleStateChanged)PublishedEvents.First()).EnvironmentKey.Should().Be(_environmentKey);
        }

        private void ThenTheToggleIdIsSaved()
        {
            ((ToggleStateChanged)PublishedEvents.First()).ToggleId.Should().Be(_toggleId);
        }

        private void ThenTheToggleStateIsSaved()
        {
            ((ToggleStateChanged)PublishedEvents.First()).Value.Should().Be(_toggleState);
        }
    }
}
