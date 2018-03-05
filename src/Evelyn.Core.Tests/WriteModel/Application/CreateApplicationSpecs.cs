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

    public class CreateApplicationSpecs : ApplicationCommandHandlerSpecs<CreateApplication>
    {
        private string _accountId;
        private Guid _applicationId;
        private string _applicationName;

        [Fact]
        public void ApplicationDoesNotExist()
        {
            this.When(_ => WhenWeCreateAnApplication())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsApplicationCreated())
                .And(_ => ThenTheUserIdIsSaved())
                .And(_ => ThenTheAccountIdIsSaved())
                .And(_ => ThenTheNameIsSaved())
                .BDDfy();
        }

        ////[Fact]
        ////public void ApplicationAlreadyExists()
        ////{
        ////    this.Given(_ => GivenWeHaveAlreadyCreateAnApplication())
        ////        .And(_ => GivenACreateApplicationCommand())
        ////        .When(_ => WhenTheCommandIsHandled())
        ////        .Then(_ => ThenNoEventIsPublished())
        ////        .BDDfy();
        ////}

        ////private void GivenWeHaveAlreadyCreatedAnApplication()
        ////{
        ////    GivenWeHaveCreatedAnApplicationWith(_applicationId);
        ////}

        private void WhenWeCreateAnApplication()
        {
            _applicationId = DataFixture.Create<Guid>();
            _applicationName = DataFixture.Create<string>();
            _accountId = DataFixture.Create<string>();

            var command = new CreateApplication(UserId, _accountId, _applicationId, _applicationName) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsApplicationCreated()
        {
            PublishedEvents.First().Should().BeOfType<ApplicationCreated>();
        }

        private void ThenTheUserIdIsSaved()
        {
            ((ApplicationCreated)PublishedEvents.First()).UserId.Should().Be(UserId);
        }

        private void ThenTheAccountIdIsSaved()
        {
            ((ApplicationCreated)PublishedEvents.First()).AccountId.Should().Be(_accountId);
        }

        private void ThenTheNameIsSaved()
        {
            ((ApplicationCreated)PublishedEvents.First()).Name.Should().Be(_applicationName);
        }
    }
}
