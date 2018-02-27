namespace Evelyn.Core.Tests.WriteModel.Application
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Commands;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class SetToggleStateSpecs : ApplicationCommandHandlerSpecs<ChangeToggleState>
    {
        private Guid _applicationId;

        private Guid _environmentId;

        private Guid _toggleId;
        private string _toggleName;
        private string _toggleKey;
        private string _toggleState;

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .When(_ => WhenWeChangeTheValueOfAToggleThatDoesntExist())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAnEnvironmentDoesNotExistExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ToggleDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .When(_ => WhenWeChangeTheValueOfAToggleThatDoesntExist())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAToggleDoesNotExistExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void InvalidToggleState()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
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
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeChangeTheToggleState())
                .Then(_ => ThenThePublishedEventIsToggledValueChanged())
                .And(_ => ThenTheApplicationIdIsSaved())
                .And(_ => ThenTheEnvironmentIdIsSaved())
                .And(_ => ThenTheToggleIdIsSaved())
                .And(_ => ThenTheToggleStateIsSaved())
                .BDDfy();
        }

        private void GivenWeHaveCreatedAnApplication()
        {
            _applicationId = DataFixture.Create<Guid>();

            GivenWeHaveCreatedAnApplicationWith(_applicationId);
        }

        private void GivenWeHaveCreatedAnEnvironment()
        {
            _environmentId = DataFixture.Create<Guid>();

            GivenWeHaveAddedAnEnvironmentWith(_applicationId, _environmentId);
        }

        private void GivenWeHaveAddedAToggle()
        {
            _toggleId = DataFixture.Create<Guid>();
            _toggleName = DataFixture.Create<string>();
            _toggleKey = DataFixture.Create<string>();

            HistoricalEvents.Add(new ToggleAdded(_applicationId, _toggleId, _toggleName, _toggleKey) { Version = HistoricalEvents.Count + 1 });
        }

        private void GivenWeHaveChangedTheToggleState()
        {
            HistoricalEvents.Add(new ToggleStateChanged(_applicationId, _environmentId, _toggleId, DataFixture.Create<bool>().ToString()) { Version = HistoricalEvents.Count + 1 });
        }

        private void WhenWeChangeTheValueOfAToggleThatDoesntExist()
        {
            _toggleId = DataFixture.Create<Guid>();
            _toggleState = DataFixture.Create<bool>().ToString();

            var command = new ChangeToggleState(_applicationId, _environmentId, _toggleId, _toggleState) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeChangeTheToggleStateToAnInvalidValue()
        {
            _toggleState = DataFixture.Create<string>();

            var command = new ChangeToggleState(_applicationId, _environmentId, _toggleId, _toggleState) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeChangeTheToggleState()
        {
            _toggleState = DataFixture.Create<bool>().ToString();

            var command = new ChangeToggleState(_applicationId, _environmentId, _toggleId, _toggleState) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void ThenAnApplicationDoesNotExistExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is no application with the ID {_applicationId}");
        }

        private void ThenAnEnvironmentDoesNotExistExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is no environment with the ID {_environmentId}");
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

        private void ThenTheApplicationIdIsSaved()
        {
            ((ToggleStateChanged)PublishedEvents.First()).Id.Should().Be(_applicationId);
        }

        private void ThenTheEnvironmentIdIsSaved()
        {
            ((ToggleStateChanged)PublishedEvents.First()).EnvironmentId.Should().Be(_environmentId);
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
