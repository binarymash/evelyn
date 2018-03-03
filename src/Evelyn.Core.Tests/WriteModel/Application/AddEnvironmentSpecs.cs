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

    public class AddEnvironmentSpecs : ApplicationCommandHandlerSpecs<AddEnvironment>
    {
        private Guid _applicationId;

        private Guid _newEnvironmentId;
        private string _newEnvironmentName;

        private Guid _existingEnvironmentId;
        private string _existingEnvironmentName;

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .When(_ => WhenWeAddAnEnvironment())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsEnvironmentAdded())
                .And(_ => ThenTheNameIsSaved())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAlreadyExistWithSameId()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .When(_ => WhenWeAddAnotherEnvironmentWithTheSameId())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateEnvironmentIdExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAlreadyExistWithSameName()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .When(_ => WhenWeAddAnotherEnvironmentWithTheSameName())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateEnvironmentNameExceptionIsThrown())
                .BDDfy();
        }

        private void GivenWeHaveCreatedAnApplication()
        {
            _applicationId = DataFixture.Create<Guid>();
            GivenWeHaveCreatedAnApplicationWith(_applicationId);
        }

        private void GivenWeHaveAddedAnEnvironment()
        {
            _existingEnvironmentId = DataFixture.Create<Guid>();
            _existingEnvironmentName = DataFixture.Create<string>();

            HistoricalEvents.Add(new EnvironmentAdded(UserId, _applicationId, _existingEnvironmentId, _existingEnvironmentName) { Version = HistoricalEvents.Count });
        }

        private void WhenWeAddAnEnvironment()
        {
            _newEnvironmentId = DataFixture.Create<Guid>();
            _newEnvironmentName = DataFixture.Create<string>();

            var command = new AddEnvironment(UserId, _applicationId, _newEnvironmentId, _newEnvironmentName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameId()
        {
            _newEnvironmentId = _existingEnvironmentId;
            _newEnvironmentName = DataFixture.Create<string>();

            var command = new AddEnvironment(UserId, _applicationId, _newEnvironmentId, _newEnvironmentName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameName()
        {
            _newEnvironmentId = DataFixture.Create<Guid>();
            _newEnvironmentName = _existingEnvironmentName;

            var command = new AddEnvironment(UserId, _applicationId, _newEnvironmentId, _newEnvironmentName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsEnvironmentAdded()
        {
            PublishedEvents.First().Should().BeOfType<EnvironmentAdded>();
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
