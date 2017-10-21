namespace Evelyn.Core.Tests.WriteModel.Application
{
    using System;
    using System.Linq;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Commands;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class CreateApplicationSpecs : ApplicationCommandHandlerSpecs<CreateApplication>
    {
        private Guid _id;

        private string _name;

        private string _expectedName;

        public CreateApplicationSpecs()
        {
            _id = Guid.NewGuid();
            _name = "some name"; // autofixture
            _expectedName = _name;
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

        private void GivenWeHaveAlreadyCreateAnApplication()
        {
            GivenWeHaveCreatedAnApplicationWith(_id);
        }

        private void WhenACreateApplicationCommand()
        {
            var command = new CreateApplication(_id, _name) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsApplicationCreated()
        {
            PublishedEvents.First().ShouldBeOfType<ApplicationCreated>();
        }

        private void ThenTheNameIsSaved()
        {
            ((ApplicationCreated)PublishedEvents.First()).Name.ShouldBe(_expectedName);
        }
    }
}
