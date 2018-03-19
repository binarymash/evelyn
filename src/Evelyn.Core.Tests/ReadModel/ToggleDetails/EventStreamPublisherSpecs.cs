namespace Evelyn.Core.Tests.ReadModel.ToggleDetails
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.ToggleDetails;
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
        public void ToggleAdded()
        {
            this.When(_ => WhenToggleAddedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<ToggleDetailsDto>())
                .BDDfy();
        }

        private async Task WhenToggleAddedEventIsHandled()
        {
            var message = Fixture.Create<ToggleAdded>();
            await Publisher.Handle(message);
            Message = message;
        }
    }
}
