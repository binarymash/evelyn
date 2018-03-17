namespace Evelyn.Core.Tests.ReadModel.EnvironmentState
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.EnvironmentState;
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
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<EnvironmentStateDto>())
                .BDDfy();
        }

        [Fact]
        public void ToggleAdded()
        {
            this.When(_ => WhenToggleAddedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<EnvironmentStateDto>())
                .BDDfy();
        }

        [Fact]
        public void ToggleStateChanged()
        {
            this.When(_ => WhenToggleStateChangedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<EnvironmentStateDto>())
                .BDDfy();
        }

        private async Task WhenEnvironmentAddedEventIsHandled()
        {
            var message = Fixture.Create<EnvironmentAdded>();
            await Publisher.Handle(message);
            Message = message;
        }

        private async Task WhenToggleAddedEventIsHandled()
        {
            var message = Fixture.Create<ToggleAdded>();
            await Publisher.Handle(message);
            Message = message;
        }

        private async Task WhenToggleStateChangedEventIsHandled()
        {
            var message = Fixture.Create<ToggleStateChanged>();
            await Publisher.Handle(message);
            Message = message;
        }
    }
}
