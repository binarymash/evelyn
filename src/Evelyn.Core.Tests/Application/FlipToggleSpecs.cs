namespace Evelyn.Core.Tests.Application
{
    using System;

    using Evelyn.Core.WriteModel.Commands;
    using TestStack.BDDfy;
    using Xunit;
    using System.Linq;
    using Evelyn.Core.ReadModel.Events;
    using Shouldly;

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
            _applicationId = Guid.NewGuid();
            GivenWeHaveCreatedAnApplicationWith(_applicationId);
        }

        private void GivenWeHaveCreatedAnEnvironment()
        {
            _environmentId = Guid.NewGuid();
            GivenWeHaveAddedAnEnvironmentWith(_applicationId, _environmentId);
        }

        private void GivenWeHaveAddedAToggle()
        {
            _toggleId = Guid.NewGuid();
            _toggleName = "some name";
            _toggleKey = "some key";
            HistoricalEvents.Add(new ToggleAdded(_applicationId, _toggleId, _toggleName, _toggleKey) { Version = HistoricalEvents.Count + 1 });
        }

        private void GivenWeHaveFlippedTheToggle()
        {
            HistoricalEvents.Add(new ToggleFlipped(_applicationId, _environmentId, _toggleId, true) { Version = HistoricalEvents.Count + 1 });
        }

        private void WhenWeFlipAToggleThatDoesntExist()
        {
            _toggleId = Guid.NewGuid();
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
            PublishedEvents.First().ShouldBeOfType<ToggleFlipped>();
        }

        private void ThenTheApplicationIdIsSaved()
        {
            ((ToggleFlipped)PublishedEvents.First()).Id.ShouldBe(_applicationId);
        }

        private void ThenTheEnvironmentIdIsSaved()
        {
            ((ToggleFlipped)PublishedEvents.First()).EnvironmentId.ShouldBe(_environmentId);
        }

        private void ThenTheToggleIdIsSaved()
        {
            ((ToggleFlipped)PublishedEvents.First()).ToggleId.ShouldBe(_toggleId);
        }

        private void ThenTheToggleValueIdIsSavedAsTrue()
        {
            ((ToggleFlipped)PublishedEvents.First()).Value.ShouldBeTrue();
        }

        private void ThenTheToggleValueIdIsSavedAsFalse()
        {
            ((ToggleFlipped)PublishedEvents.First()).Value.ShouldBeFalse();
        }
    }
}
