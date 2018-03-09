namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Project.Commands;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class AddEnvironmentSpecs : ProjectCommandHandlerSpecs<AddEnvironment>
    {
        private Guid _projectId;

        private string _newEnvironmentKey;

        private string _existingEnvironmentKey;

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .When(_ => WhenWeAddAnEnvironment())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsEnvironmentAdded())
                .And(_ => ThenTheUserIdIsSaved())
                .And(_ => ThenTheNameIsSaved())
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

        private void GivenWeHaveAddedAnEnvironment()
        {
            _existingEnvironmentKey = DataFixture.Create<string>();

            HistoricalEvents.Add(new EnvironmentAdded(UserId, _projectId, _existingEnvironmentKey) { Version = HistoricalEvents.Count });
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

        private void ThenThePublishedEventIsEnvironmentAdded()
        {
            PublishedEvents.First().Should().BeOfType<EnvironmentAdded>();
        }

        private void ThenTheUserIdIsSaved()
        {
            ((EnvironmentAdded)PublishedEvents.First()).UserId.Should().Be(UserId);
        }

        private void ThenTheNameIsSaved()
        {
            ((EnvironmentAdded)PublishedEvents.First()).Key.Should().Be(_newEnvironmentKey);
        }

        private void ThenADuplicateEnvironmentKeyExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the key {_newEnvironmentKey}");
        }
    }
}
