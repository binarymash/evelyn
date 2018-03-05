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

    public class AddToggleSpecs : ProjectCommandHandlerSpecs<AddToggle>
    {
        private Guid _projectId;

        private Guid _newToggleId;
        private string _newToggleName;
        private string _newToggleKey;

        private Guid _existingToggleId;
        private string _existingToggleName;
        private string _existingToggleKey;

        [Fact]
        public void ToggleDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .When(_ => WhenWeAddAToggle())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsToggleAdded())
                .And(_ => ThenTheUserIdIsSaved())
                .And(_ => ThenTheNameIsSaved())
                .And(_ => ThenTheKeyIsSaved())
                .BDDfy();
        }

        [Fact]
        public void ToggleAlreadyExistWithSameId()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeAddAnotherToggleWithTheSameId())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateToggleIdExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ToggleAlreadyExistWithSameKey()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .When(_ => WhenWeAddAnotherToggleWithTheSameKey())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateToggleKeyExceptionIsThrown())
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

        private void GivenWeHaveAddedAToggle()
        {
            _existingToggleId = DataFixture.Create<Guid>();
            _existingToggleName = DataFixture.Create<string>();
            _existingToggleKey = DataFixture.Create<string>();

            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _existingToggleId, _existingToggleName, _existingToggleKey) { Version = HistoricalEvents.Count });
        }

        private void WhenWeAddAToggle()
        {
            _newToggleId = DataFixture.Create<Guid>();
            _newToggleName = DataFixture.Create<string>();
            _newToggleKey = DataFixture.Create<string>();

            var command = new AddToggle(UserId, _projectId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameId()
        {
            _newToggleId = _existingToggleId;
            _newToggleName = DataFixture.Create<string>();
            _newToggleKey = DataFixture.Create<string>();

            var command = new AddToggle(UserId, _projectId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameKey()
        {
            _newToggleId = DataFixture.Create<Guid>();
            _newToggleName = DataFixture.Create<string>();
            _newToggleKey = _existingToggleKey;

            var command = new AddToggle(UserId, _projectId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameName()
        {
            _newToggleId = DataFixture.Create<Guid>();
            _newToggleName = _existingToggleName;
            _newToggleKey = DataFixture.Create<string>();

            var command = new AddToggle(UserId, _projectId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsToggleAdded()
        {
            PublishedEvents.First().Should().BeOfType<ToggleAdded>();
        }

        private void ThenTheUserIdIsSaved()
        {
            ((ToggleAdded)PublishedEvents.First()).UserId.Should().Be(UserId);
        }

        private void ThenTheNameIsSaved()
        {
            ((ToggleAdded)PublishedEvents.First()).Name.Should().Be(_newToggleName);
        }

        private void ThenTheKeyIsSaved()
        {
            ((ToggleAdded)PublishedEvents.First()).Key.Should().Be(_newToggleKey);
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