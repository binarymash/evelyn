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
        private readonly Fixture _fixture;

        private Guid _applicationId;

        private Guid _newEnvironmentId;
        private string _newEnvironmentName;
        private string _newEnvironmentKey;

        private Guid _existingEnvironmentId;
        private string _existingEnvironmentName;
        private string _existingEnvironmentKey;

        public AddEnvironmentSpecs()
        {
            _fixture = new Fixture();
        }

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
            _applicationId = _fixture.Create<Guid>();
            GivenWeHaveCreatedAnApplicationWith(_applicationId);
        }

        private void GivenWeHaveAddedAnEnvironment()
        {
            _existingEnvironmentId = _fixture.Create<Guid>();
            _existingEnvironmentName = _fixture.Create<string>();
            _existingEnvironmentKey = _fixture.Create<string>();

            HistoricalEvents.Add(new EnvironmentAdded(_applicationId, _existingEnvironmentId, _existingEnvironmentName, _existingEnvironmentKey) { Version = HistoricalEvents.Count + 1 });
        }

        private void WhenWeAddAnEnvironment()
        {
            _newEnvironmentId = _fixture.Create<Guid>();
            _newEnvironmentName = _fixture.Create<string>();
            _newEnvironmentKey = _fixture.Create<string>();

            var command = new AddEnvironment(_applicationId, _newEnvironmentId, _newEnvironmentName, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameId()
        {
            _newEnvironmentId = _existingEnvironmentId;
            _newEnvironmentName = _fixture.Create<string>();
            _newEnvironmentKey = _fixture.Create<string>();

            var command = new AddEnvironment(_applicationId, _newEnvironmentId, _newEnvironmentName, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameKey()
        {
            _newEnvironmentId = _fixture.Create<Guid>();
            _newEnvironmentName = _fixture.Create<string>();
            _newEnvironmentKey = _existingEnvironmentKey;

            var command = new AddEnvironment(_applicationId, _newEnvironmentId, _newEnvironmentName, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameName()
        {
            _newEnvironmentId = _fixture.Create<Guid>();
            _newEnvironmentName = _existingEnvironmentName;
            _newEnvironmentKey = _fixture.Create<string>();

            var command = new AddEnvironment(_applicationId, _newEnvironmentId, _newEnvironmentName, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count };
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

        private void ThenTheKeyIsSaved()
        {
            ((EnvironmentAdded)PublishedEvents.First()).Key.Should().Be(_newEnvironmentKey);
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
