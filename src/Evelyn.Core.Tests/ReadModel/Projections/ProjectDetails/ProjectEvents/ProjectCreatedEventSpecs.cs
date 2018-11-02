namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectCreatedEventSpecs : EventSpecs
    {
        private ProjectCreated _event;

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenThereIsNoProjection())
                .When(_ => _.WhenWeHandleAProjectCreatedEvent())
                .Then(_ => _.ThenTheProjectionIsCreatedWithTheCorrectProperties())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            _event = DataFixture.Build<ProjectCreated>()
                .With(ar => ar.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreatedWithTheCorrectProperties()
        {
            ThenTheProjectionIsCreated();

            UpdatedProjection.Created.Should().Be(_event.OccurredAt);
            UpdatedProjection.CreatedBy.Should().Be(_event.UserId);
            UpdatedProjection.LastModified.Should().Be(_event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(_event.UserId);
            UpdatedProjection.Version.Should().Be(_event.Version);

            UpdatedProjection.Name.Should().Be(_event.Name);
            UpdatedProjection.Environments.Should().BeEmpty();
            UpdatedProjection.Toggles.Should().BeEmpty();
        }
    }
}
