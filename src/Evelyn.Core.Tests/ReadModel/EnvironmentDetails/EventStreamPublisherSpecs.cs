namespace Evelyn.Core.Tests.ReadModel.EnvironmentDetails
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.EnvironmentDetails;
    using Core.WriteModel.Project.Events;
    using TestStack.BDDfy;
    using Xunit;

    public class EventStreamPublisherSpecs : EventStreamPublisherSpecs<EventStreamPublisher>
    {
        public EventStreamPublisherSpecs()
        {
            Publisher = new EventStreamPublisher(EventStreamFactory);
        }

        [Fact]
        public void EnvironmentAdded()
        {
            this.When(_ => WhenEnvironmentAddedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<EnvironmentDetailsDto>())
                .BDDfy();
        }

        private async Task WhenEnvironmentAddedEventIsHandled()
        {
            var message = Fixture.Create<EnvironmentAdded>();
            await Publisher.Handle(message);
            Message = message;
        }
    }
}
