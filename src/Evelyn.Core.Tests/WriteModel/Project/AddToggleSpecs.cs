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

        private string _existingToggleName;
        private string _existingToggleKey;

        [Fact]
        public void ToggleDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .When(_ => WhenWeAddAToggle())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsToggleAdded())
                .And(_ => ThenThereIsOneChangeOnTheAggregate())
                .And(_ => ThenTheToggleHasBeenAddedToTheAggregate())
                .And(_ => ThenTheAggregateVersionHasBeenIncreased())
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

        private void ThenThePublishedEventIsToggleAdded()
        {
            var ev = (ToggleAdded)PublishedEvents.First();

            ev.UserId.Should().Be(UserId);
            ev.Name.Should().Be(_newToggleName);
            ev.Key.Should().Be(_newToggleKey);
        }

        private void ThenADuplicateToggleKeyExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already a toggle with the key {_newToggleKey}");
        }

        private void ThenADuplicateToggleNameExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already a toggle with the name {_newToggleName}");
        }

        private void ThenTheToggleHasBeenAddedToTheAggregate()
        {
            ComparisonResult.Differences
                .Exists(d => d.PropertyName == "Toggles")
                .Should().BeTrue();

            var toggle = NewAggregate.Toggles.First(e => e.Key == _newToggleKey);

            toggle.Name.Should().Be(_newToggleName);

            toggle.Created.Should().BeOnOrAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            toggle.CreatedBy.Should().Be(UserId);

            toggle.LastModified.Should().Be(toggle.Created);
            toggle.LastModifiedBy.Should().Be(toggle.CreatedBy);
        }
    }
}
