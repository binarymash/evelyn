namespace Evelyn.Core.Tests.ReadModel.AccountProjects
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.AccountProjects;
    using Core.WriteModel.Account.Events;
    using TestStack.BDDfy;
    using Xunit;

    public class EventStreamPublisherSpecs : EventStreamPublisherSpecs<EventStreamPublisher>
    {
        public EventStreamPublisherSpecs()
        {
            Publisher = new EventStreamPublisher(EventStreamFactory);
        }

        [Fact]
        public void AccountRegistered()
        {
            this.When(_ => WhenAccountRegisteredEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<AccountProjectsDto>())
                .BDDfy();
        }

        [Fact]
        public void ProjectCreated()
        {
            this.When(_ => WhenProjectCreatedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<AccountProjectsDto>())
                .BDDfy();
        }

        private async Task WhenAccountRegisteredEventIsHandled()
        {
            var message = Fixture.Create<AccountRegistered>();
            await Publisher.Handle(message);
            Message = message;
        }

        private async Task WhenProjectCreatedEventIsHandled()
        {
            var message = Fixture.Create<ProjectCreated>();
            await Publisher.Handle(message);
            Message = message;
        }
    }
}
