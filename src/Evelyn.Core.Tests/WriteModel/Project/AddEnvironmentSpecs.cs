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

    public class AddEnvironmentSpecs : ProjectCommandHandlerSpecs<AddEnvironment>
    {
        private Guid _projectId;

        private Guid _newEnvironmentId;
        private string _newEnvironmentName;

        private Guid _existingEnvironmentId;
        private string _existingEnvironmentName;

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
        public void EnvironmentAlreadyExistWithSameId()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .When(_ => WhenWeAddAnotherEnvironmentWithTheSameId())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateEnvironmentIdExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAlreadyExistWithSameName()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .When(_ => WhenWeAddAnotherEnvironmentWithTheSameName())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateEnvironmentNameExceptionIsThrown())
                .BDDfy();
        }

        private void GivenWeHaveCreatedAProject()
        {
            _projectId = DataFixture.Create<Guid>();
            GivenWeHaveCreatedAProjectWith(_projectId);
        }

        private void GivenWeHaveAddedAnEnvironment()
        {
            _existingEnvironmentId = DataFixture.Create<Guid>();
            _existingEnvironmentName = DataFixture.Create<string>();

            HistoricalEvents.Add(new EnvironmentAdded(UserId, _projectId, _existingEnvironmentId, _existingEnvironmentName) { Version = HistoricalEvents.Count });
        }

        private void WhenWeAddAnEnvironment()
        {
            _newEnvironmentId = DataFixture.Create<Guid>();
            _newEnvironmentName = DataFixture.Create<string>();

            var command = new AddEnvironment(UserId, _projectId, _newEnvironmentId, _newEnvironmentName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameId()
        {
            _newEnvironmentId = _existingEnvironmentId;
            _newEnvironmentName = DataFixture.Create<string>();

            var command = new AddEnvironment(UserId, _projectId, _newEnvironmentId, _newEnvironmentName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameName()
        {
            _newEnvironmentId = DataFixture.Create<Guid>();
            _newEnvironmentName = _existingEnvironmentName;

            var command = new AddEnvironment(UserId, _projectId, _newEnvironmentId, _newEnvironmentName) { ExpectedVersion = HistoricalEvents.Count - 1 };
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
            ((EnvironmentAdded)PublishedEvents.First()).Name.Should().Be(_newEnvironmentName);
        }

        private void ThenADuplicateEnvironmentIdExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the ID {_newEnvironmentId}");
        }

        private void ThenADuplicateEnvironmentNameExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the name {_newEnvironmentName}");
        }
    }
}
