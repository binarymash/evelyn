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

    public class AddEnvironmentSpecs : ProjectCommandHandlerSpecs<AddEnvironment>
    {
        private Guid _projectId;
        private string _newEnvironmentKey;
        private string _existingEnvironmentKey;
        private string _toggleKey;
        private string _toggleName;

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggleToTheProject())
                .When(_ => WhenWeAddAnEnvironment())
                .Then(_ => ThenTwoEventsArePublished())
                .And(_ => ThenAnEnvironmentAddedEventIsPublished())
                .And(_ => ThenAnEnvironmentStateAddedEventIsPublished())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAlreadyExistWithSameKey()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .When(_ => WhenWeAddAnotherEnvironmentWithTheSameKey())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateEnvironmentKeyExceptionIsThrown())
                .BDDfy();
        }

        private void GivenWeHaveCreatedAProject()
        {
            _projectId = DataFixture.Create<Guid>();
            GivenWeHaveCreatedAProjectWith(_projectId);
        }

        private void GivenWeHaveAddedAToggleToTheProject()
        {
            _toggleKey = DataFixture.Create<string>();
            _toggleName = DataFixture.Create<string>();

            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _toggleKey, _toggleName, DateTime.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void GivenWeHaveAddedAnEnvironment()
        {
            _existingEnvironmentKey = DataFixture.Create<string>();

            HistoricalEvents.Add(new EnvironmentAdded(UserId, _projectId, _existingEnvironmentKey, DateTime.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void WhenWeAddAnEnvironment()
        {
            _newEnvironmentKey = DataFixture.Create<string>();

            var command = new AddEnvironment(UserId, _projectId, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameKey()
        {
            _newEnvironmentKey = _existingEnvironmentKey;

            var command = new AddEnvironment(UserId, _projectId, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void ThenAnEnvironmentAddedEventIsPublished()
        {
            var @event = (EnvironmentAdded)PublishedEvents.First(ev => ev is EnvironmentAdded);
            @event.UserId.Should().Be(UserId);
            @event.Key.Should().Be(_newEnvironmentKey);
            @event.OccurredAt.Should().BeCloseTo(DateTimeOffset.UtcNow, 100);
        }

        private void ThenAnEnvironmentStateAddedEventIsPublished()
        {
            var @event = (EnvironmentStateAdded)PublishedEvents.First(ev => ev is EnvironmentStateAdded);
            @event.UserId.Should().Be(UserId);
            @event.EnvironmentKey.Should().Be(_newEnvironmentKey);
            @event.ToggleStates.ToList().Exists(ts => ts.Key == _toggleKey && ts.Value == default(bool).ToString());
            @event.OccurredAt.Should().BeCloseTo(DateTimeOffset.UtcNow, 100);
        }

        private void ThenADuplicateEnvironmentKeyExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the key {_newEnvironmentKey}");
        }
    }
}
