namespace Evelyn.Core.Tests.WriteModel.Application
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Commands;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class CreateApplicationSpecs : ApplicationCommandHandlerSpecs<CreateApplication>
    {
        private readonly Fixture _fixture;

        private Guid _applicationId;
        private string _applicationName;

        public CreateApplicationSpecs()
        {
            _fixture = new Fixture();
        }

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
            _applicationId = _fixture.Create<Guid>();
            _applicationName = _fixture.Create<string>();

            var command = new CreateApplication(_applicationId, _applicationName) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsApplicationCreated()
        {
            PublishedEvents.First().ShouldBeOfType<ApplicationCreated>();
        }

        private void ThenTheNameIsSaved()
        {
            ((ApplicationCreated)PublishedEvents.First()).Name.ShouldBe(_applicationName);
        }
    }
}
