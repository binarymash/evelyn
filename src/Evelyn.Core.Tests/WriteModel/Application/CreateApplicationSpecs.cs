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
        private Guid _applicationId;
        private string _applicationName;

        [Fact]
        public void ApplicationDoesNotExist()
        {
            this.When(_ => WhenACreateApplicationCommand())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsApplicationCreated())
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

        private void WhenACreateApplicationCommand()
        {
            _applicationId = DataFixture.Create<Guid>();
            _applicationName = DataFixture.Create<string>();

            var command = new CreateApplication(_applicationId, _applicationName) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsApplicationCreated()
        {
            PublishedEvents.First().Should().BeOfType<ApplicationCreated>();
        }

        private void ThenTheNameIsSaved()
        {
            ((ApplicationCreated)PublishedEvents.First()).Name.Should().Be(_applicationName);
        }
    }
}
