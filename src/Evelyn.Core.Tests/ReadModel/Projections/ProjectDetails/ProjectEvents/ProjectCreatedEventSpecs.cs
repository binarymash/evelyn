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

    public class ProjectCreatedEventSpecs : ProjectionHarness<ProjectCreated>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectionIsCreated())
                .And(_ => ThenTheAuditIsCreated())
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

        private void ThenTheProjectionIsCreated()
        {
            ProjectionStore.Received().Create(ProjectDetailsDto.StoreKey(ProjectId), UpdatedProjection);

            UpdatedProjection.Name.Should().Be(Event.Name);
            UpdatedProjection.Environments.Should().BeEmpty();
            UpdatedProjection.Toggles.Should().BeEmpty();
        }

        private void ThenTheProjectAuditIsCreated()
        {
            UpdatedProjection.ProjectAudit.Created.Should().Be(Event.OccurredAt);
            UpdatedProjection.ProjectAudit.CreatedBy.Should().Be(Event.UserId);
            UpdatedProjection.ProjectAudit.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.ProjectAudit.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.ProjectAudit.Version.Should().Be(Event.Version);
        }
    }
}
