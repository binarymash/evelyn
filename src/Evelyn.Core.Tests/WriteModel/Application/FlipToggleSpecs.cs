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

    public class FlipToggleSpecs : ApplicationCommandHandlerSpecs<FlipToggle>
    {
        private Guid _applicationId;

        private Guid _environmentId;

        private Guid _toggleId;
        private string _toggleName;
        private string _toggleKey;

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .When(_ => WhenWeFlipAToggleThatDoesntExist())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAnEnvironmentDoesNotExistExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ToggleDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .When(_ => WhenWeFlipAToggleThatDoesntExist())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAToggleDoesNotExistExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ToggleIsOff()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeFlipTheToggle())
                .Then(_ => ThenThePublishedEventIsToggledFlipped())
                .And(_ => ThenTheApplicationIdIsSaved())
                .And(_ => ThenTheEnvironmentIdIsSaved())
                .And(_ => ThenTheToggleIdIsSaved())
                .And(_ => ThenTheToggleValueIdIsSavedAsTrue())
                .BDDfy();
        }

        [Fact]
        public void ToggleIsOn()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveCreatedAnEnvironment())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenWeHaveFlippedTheToggle())
                .When(_ => WhenWeFlipTheToggle())
                .Then(_ => ThenThePublishedEventIsToggledFlipped())
                .And(_ => ThenTheApplicationIdIsSaved())
                .And(_ => ThenTheEnvironmentIdIsSaved())
                .And(_ => ThenTheToggleIdIsSaved())
                .And(_ => ThenTheToggleValueIdIsSavedAsFalse())
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

        private void GivenWeHaveFlippedTheToggle()
        {
            HistoricalEvents.Add(new ToggleFlipped(_applicationId, _environmentId, _toggleId, true) { Version = HistoricalEvents.Count + 1 });
        }

        private void WhenWeFlipAToggleThatDoesntExist()
        {
            _toggleId = DataFixture.Create<Guid>();

            var command = new FlipToggle(_applicationId, _environmentId, _toggleId) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeFlipTheToggle()
        {
            var command = new FlipToggle(_applicationId, _environmentId, _toggleId) { ExpectedVersion = HistoricalEvents.Count };
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

        private void ThenThePublishedEventIsToggledFlipped()
        {
            PublishedEvents.First().Should().BeOfType<ToggleFlipped>();
        }

        private void ThenTheApplicationIdIsSaved()
        {
            ((ToggleFlipped)PublishedEvents.First()).Id.Should().Be(_applicationId);
        }

        private void ThenTheEnvironmentIdIsSaved()
        {
            ((ToggleFlipped)PublishedEvents.First()).EnvironmentId.Should().Be(_environmentId);
        }

        private void ThenTheToggleIdIsSaved()
        {
            ((ToggleFlipped)PublishedEvents.First()).ToggleId.Should().Be(_toggleId);
        }

        private void ThenTheToggleValueIdIsSavedAsTrue()
        {
            ((ToggleFlipped)PublishedEvents.First()).Value.Should().BeTrue();
        }

        private void ThenTheToggleValueIdIsSavedAsFalse()
        {
            ((ToggleFlipped)PublishedEvents.First()).Value.Should().BeFalse();
        }
    }
}
