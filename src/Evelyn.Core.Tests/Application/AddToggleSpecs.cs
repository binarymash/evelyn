namespace Evelyn.Core.Tests.Application
{
    using System;
    using System.Linq;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Commands;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class AddToggleSpecs : ApplicationCommandHandlerSpecs<AddToggle>
    {
        private Guid _applicationId;
        private Guid _newToggleId;
        private string _newToggleName;
        private string _newToggleKey;
        private Guid _existingToggleId;
        private string _existingToggleName;
        private string _existingToggleKey;

        [Fact]
        public void ToggleDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .When(_ => WhenWeAddAToggle())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsToggleAdded())
                .And(_ => ThenTheNameIsSaved())
                .And(_ => ThenTheKeyIsSaved())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAlreadyExistWithSameId()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeAddAnotherToggleWithTheSameId())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateToggleIdExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAlreadyExistWithSameKey()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeAddAnotherToggleWithTheSameKey())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateToggleKeyExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAlreadyExistWithSameName()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeAddAnotherToggleWithTheSameName())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateToggleNameExceptionIsThrown())
                .BDDfy();
        }

        private void GivenWeHaveCreatedAnApplication()
        {
            _applicationId = Guid.NewGuid();
            GivenWeHaveCreatedAnApplicationWith(_applicationId);
        }

        private void GivenWeHaveAddedAToggle()
        {
            _existingToggleId = Guid.NewGuid();
            _existingToggleName = "some name";
            _existingToggleKey = "some key";
            HistoricalEvents.Add(new ToggleAdded(_applicationId, _existingToggleId, _existingToggleName, _existingToggleKey) { Version = HistoricalEvents.Count + 1 });
        }

        private void WhenWeAddAToggle()
        {
            _newToggleId = Guid.NewGuid();
            _newToggleName = "some name";
            _newToggleKey = "some key";
            var command = new AddToggle(_applicationId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameId()
        {
            _newToggleId = _existingToggleId;
            _newToggleName = "some other name";
            _newToggleKey = "some other key";
            var command = new AddToggle(_applicationId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameKey()
        {
            _newToggleId = Guid.NewGuid();
            _newToggleName = "some other name";
            _newToggleKey = _existingToggleKey;
            var command = new AddToggle(_applicationId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameName()
        {
            _newToggleId = Guid.NewGuid();
            _newToggleName = _existingToggleName;
            _newToggleKey = "some other name";
            var command = new AddToggle(_applicationId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsToggleAdded()
        {
            PublishedEvents.First().ShouldBeOfType<ToggleAdded>();
        }

        private void ThenTheNameIsSaved()
        {
            ((ToggleAdded)PublishedEvents.First()).Name.ShouldBe(_newToggleName);
        }

        private void ThenTheKeyIsSaved()
        {
            ((ToggleAdded)PublishedEvents.First()).Key.ShouldBe(_newToggleKey);
        }

        private void ThenADuplicateToggleIdExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already a toggle with the ID {_newToggleId}");
        }

        private void ThenADuplicateToggleKeyExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already a toggle with the key {_newToggleKey}");
        }

        private void ThenADuplicateToggleNameExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already a toggle with the name {_newToggleName}");
        }
    }
}
