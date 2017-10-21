namespace Evelyn.Core.Tests.WriteModel.Application
{
    using System;
    using System.Linq;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Commands;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class AddEnvironmentSpecs : ApplicationCommandHandlerSpecs<AddEnvironment>
    {
        private Guid _applicationId;
        private Guid _newEnvironmentId;
        private string _newEnvironmentName;
        private string _newEnvironmentKey;
        private Guid _existingEnvironmentId;
        private string _existingEnvironmentName;
        private string _existingEnvironmentKey;

        [Fact]
        public void EnvironmentDoesntExist()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .When(_ => WhenWeAddAnEnvironment())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsEnvironmentAdded())
                .And(_ => ThenTheNameIsSaved())
                .And(_ => ThenTheKeyIsSaved())
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
        public void EnvironmentAlreadyExistWithSameKey()
        {
            this.Given(_ => GivenWeHaveCreatedAnApplication())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .When(_ => WhenWeAddAnotherEnvironmentWithTheSameKey())
                .Then(_ => ThenNoEventIsPublished())
                .Then(_ => ThenADuplicateEnvironmentKeyExceptionIsThrown())
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
            _applicationId = Guid.NewGuid();
            GivenWeHaveCreatedAnApplicationWith(_applicationId);
        }

        private void GivenWeHaveAddedAnEnvironment()
        {
            _existingEnvironmentId = Guid.NewGuid();
            _existingEnvironmentName = "some name";
            _existingEnvironmentKey = "some key";
            HistoricalEvents.Add(new EnvironmentAdded(_applicationId, _existingEnvironmentId, _existingEnvironmentName, _existingEnvironmentKey) { Version = HistoricalEvents.Count + 1 });
        }

        private void WhenWeAddAnEnvironment()
        {
            _newEnvironmentId = Guid.NewGuid();
            _newEnvironmentName = "some name";
            _newEnvironmentKey = "some key";
            var command = new AddEnvironment(_applicationId, _newEnvironmentId, _newEnvironmentName, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameId()
        {
            _newEnvironmentId = _existingEnvironmentId;
            _newEnvironmentName = "some other name";
            _newEnvironmentKey = "some other key";
            var command = new AddEnvironment(_applicationId, _newEnvironmentId, _newEnvironmentName, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameKey()
        {
            _newEnvironmentId = Guid.NewGuid();
            _newEnvironmentName = "some other name";
            _newEnvironmentKey = _existingEnvironmentKey;
            var command = new AddEnvironment(_applicationId, _newEnvironmentId, _newEnvironmentName, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameName()
        {
            _newEnvironmentId = Guid.NewGuid();
            _newEnvironmentName = _existingEnvironmentName;
            var command = new AddEnvironment(_applicationId, _newEnvironmentId, _newEnvironmentName, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsEnvironmentAdded()
        {
            PublishedEvents.First().ShouldBeOfType<EnvironmentAdded>();
        }

        private void ThenTheNameIsSaved()
        {
            ((EnvironmentAdded)PublishedEvents.First()).Name.ShouldBe(_newEnvironmentName);
        }

        private void ThenTheKeyIsSaved()
        {
            ((EnvironmentAdded)PublishedEvents.First()).Key.ShouldBe(_newEnvironmentKey);
        }

        private void ThenADuplicateEnvironmentIdExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the ID {_newEnvironmentId}");
        }

        private void ThenADuplicateEnvironmentKeyExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the key {_newEnvironmentKey}");
        }

        private void ThenADuplicateEnvironmentNameExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the name {_newEnvironmentName}");
        }
    }
}
