namespace Evelyn.Core.Tests.ReadModel.ProjectDetails
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.ProjectDetails;
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
        public void ProjectCreated()
        {
            this.When(_ => WhenProjectCreatedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<ProjectDetailsDto>())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAdded()
        {
            this.When(_ => WhenEnvironmentAddedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<ProjectDetailsDto>())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentDeleted()
        {
            this.When(_ => WhenEnvironmentDeletedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<ProjectDetailsDto>())
                .BDDfy();
        }

        [Fact]
        public void ToggleAdded()
        {
            this.When(_ => WhenToggleAddedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<ProjectDetailsDto>())
                .BDDfy();
        }

        [Fact]
        public void ToggleDeleted()
        {
            this.When(_ => WhenToggleDeletedEventIsHandled())
                .Then(_ => ThenTheEventIsAddedToTheStreamFor<ProjectDetailsDto>())
                .BDDfy();
        }

        private async Task WhenProjectCreatedEventIsHandled()
        {
            var message = Fixture.Create<ProjectCreated>();
            await Publisher.Handle(message);
            Message = message;
        }

        private async Task WhenEnvironmentAddedEventIsHandled()
        {
            var message = Fixture.Create<EnvironmentAdded>();
            await Publisher.Handle(message);
            Message = message;
        }

        private async Task WhenEnvironmentDeletedEventIsHandled()
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

        private async Task WhenToggleDeletedEventIsHandled()
        {
            var message = Fixture.Create<ToggleAdded>();
            await Publisher.Handle(message);
            Message = message;
        }
    }
}
