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

    public class AddToggleSpecs : ApplicationCommandHandlerSpecs<AddToggle>
    {
        private readonly Fixture _fixture;

        private Guid _applicationId;

        private Guid _newToggleId;
        private string _newToggleName;
        private string _newToggleKey;

        private Guid _existingToggleId;
        private string _existingToggleName;
        private string _existingToggleKey;

        public AddToggleSpecs()
        {
            _fixture = new Fixture();
        }

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
            _applicationId = _fixture.Create<Guid>();

            GivenWeHaveCreatedAnApplicationWith(_applicationId);
        }

        private void GivenWeHaveAddedAToggle()
        {
            _existingToggleId = _fixture.Create<Guid>();
            _existingToggleName = _fixture.Create<string>();
            _existingToggleKey = _fixture.Create<string>();

            HistoricalEvents.Add(new ToggleAdded(_applicationId, _existingToggleId, _existingToggleName, _existingToggleKey) { Version = HistoricalEvents.Count + 1 });
        }

        private void WhenWeAddAToggle()
        {
            _newToggleId = _fixture.Create<Guid>();
            _newToggleName = _fixture.Create<string>();
            _newToggleKey = _fixture.Create<string>();

            var command = new AddToggle(_applicationId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameId()
        {
            _newToggleId = _existingToggleId;
            _newToggleName = _fixture.Create<string>();
            _newToggleKey = _fixture.Create<string>();

            var command = new AddToggle(_applicationId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameKey()
        {
            _newToggleId = _fixture.Create<Guid>();
            _newToggleName = _fixture.Create<string>();
            _newToggleKey = _existingToggleKey;

            var command = new AddToggle(_applicationId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherToggleWithTheSameName()
        {
            _newToggleId = _fixture.Create<Guid>();
            _newToggleName = _existingToggleName;
            _newToggleKey = _fixture.Create<string>();

            var command = new AddToggle(_applicationId, _newToggleId, _newToggleName, _newToggleKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsToggleAdded()
        {
            PublishedEvents.First().Should().BeOfType<ToggleAdded>();
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
