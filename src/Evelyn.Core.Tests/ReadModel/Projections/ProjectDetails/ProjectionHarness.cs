namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Evelyn.Core.WriteModel;
    using FluentAssertions;

    public abstract class ProjectionHarness<TEvent> : ProjectionsHarness<ProjectDetailsDto, ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        public ProjectionHarness()
        {
            ProjectionBuilder = new ProjectionBuilder(ProjectionStore);
        }

        protected Guid ProjectId { get; private set; }

        protected void GivenThereIsNoProjection()
        {
            ProjectId = DataFixture.Create<Guid>();
        }

        protected void GivenTheProjectionExists()
        {
            OriginalProjection = DataFixture.Create<ProjectDetailsDto>();
            ProjectId = OriginalProjection.Id;
        }

        protected void GivenThereAreEnvironmentsOnTheProjection()
        {
            OriginalProjection.AddEnvironment(
                DataFixture.Create<EventAuditDto>(),
                DataFixture.Create<string>(),
                DataFixture.Create<string>());
        }

        protected void GivenThereAreTogglesOnTheProjection()
        {
            OriginalProjection.AddToggle(
                DataFixture.Create<EventAuditDto>(),
                DataFixture.Create<string>(),
                DataFixture.Create<string>());
        }

        protected void ThenTheProjectAuditIsUpdated()
        {
            UpdatedProjection.ProjectAudit.Created.Should().Be(OriginalProjection.ProjectAudit.Created);
            UpdatedProjection.ProjectAudit.CreatedBy.Should().Be(OriginalProjection.ProjectAudit.CreatedBy);
            UpdatedProjection.ProjectAudit.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.ProjectAudit.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.ProjectAudit.Version.Should().Be(Event.Version);
        }
    }
}
