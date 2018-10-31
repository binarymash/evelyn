namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class EnvironmentAddedSpecs : EventSpecs
    {
        private ProjectEvents.EnvironmentAdded _event;

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectionIsCreated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            _event = DataFixture.Build<ProjectEvents.EnvironmentAdded>()
               .With(e => e.Id, ProjectId)
               .With(e => e.Key, EnvironmentKey)
               .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreated()
        {
            UpdatedProjection.ProjectId.Should().Be(_event.Id);
            UpdatedProjection.Created.Should().Be(_event.OccurredAt);
            UpdatedProjection.CreatedBy.Should().Be(_event.UserId);

            UpdatedProjection.LastModified.Should().Be(_event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(_event.UserId);
            UpdatedProjection.Version.Should().Be(0);

            UpdatedProjection.Key.Should().Be(_event.Key);
            UpdatedProjection.Name.Should().Be(_event.Name);
        }
    }
}
