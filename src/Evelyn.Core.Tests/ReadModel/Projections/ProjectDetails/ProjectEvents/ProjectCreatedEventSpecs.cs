namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectCreatedEventSpecs : ProjectionHarness<ProjectCreated>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectionIsCreated())
                .And(_ => ThenTheAuditIsCreated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(Event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            Event = DataFixture.Build<ProjectCreated>()
                .With(ar => ar.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreated()
        {
            ProjectionStore.Received().Create(ProjectDetailsDto.StoreKey(ProjectId), UpdatedProjection);

            UpdatedProjection.Name.Should().Be(Event.Name);
            UpdatedProjection.Environments.Should().BeEmpty();
            UpdatedProjection.Toggles.Should().BeEmpty();
        }
    }
}
