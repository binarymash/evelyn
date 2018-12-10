namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectCreatedEventSpecs : ProjectionBuilderHarness<ProjectCreated>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .And(_ => ThenTheProjectionContainsTheProjectDetails())
                .And(_ => ThenTheProjectAuditIsCreated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            Event = DataFixture.Build<ProjectCreated>()
                .With(ar => ar.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionContainsTheProjectDetails()
        {
            ProjectionStore.Received().Create(Projection.StoreKey(ProjectId), UpdatedProjection);

            var project = UpdatedProjection.Project;
            project.Name.Should().Be(Event.Name);
            project.Environments.Should().BeEmpty();
            project.Toggles.Should().BeEmpty();
        }

        private void ThenTheProjectAuditIsCreated()
        {
            var audit = UpdatedProjection.Project.Audit;
            audit.Created.Should().Be(Event.OccurredAt);
            audit.CreatedBy.Should().Be(Event.UserId);
            audit.LastModified.Should().Be(Event.OccurredAt);
            audit.LastModifiedBy.Should().Be(Event.UserId);
            audit.Version.Should().Be(Event.Version);
        }
    }
}
