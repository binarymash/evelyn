namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Evelyn.Core.WriteModel;
    using FluentAssertions;

    public abstract class ProjectionBuilderHarness<TEvent> : ProjectionBuilderHarness<Projection, ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        public ProjectionBuilderHarness()
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
            OriginalProjection = DataFixture.Create<Projection>();
            ProjectId = OriginalProjection.Project.Id;
        }

        protected void GivenThereAreEnvironmentsOnTheProject()
        {
            OriginalProjection.Project.AddEnvironment(
                DataFixture.Create<EventAuditDto>(),
                DataFixture.Create<string>(),
                DataFixture.Create<string>());
        }

        protected void GivenThereAreTogglesOnTheProject()
        {
            OriginalProjection.Project.AddToggle(
                DataFixture.Create<EventAuditDto>(),
                DataFixture.Create<string>(),
                DataFixture.Create<string>());
        }

        protected void ThenTheProjectAuditIsUpdated()
        {
            var audit = UpdatedProjection.Project.Audit;

            audit.Created.Should().Be(OriginalProjection.Project.Audit.Created);
            audit.CreatedBy.Should().Be(OriginalProjection.Project.Audit.CreatedBy);
            audit.LastModified.Should().Be(Event.OccurredAt);
            audit.LastModifiedBy.Should().Be(Event.UserId);
            audit.Version.Should().Be(Event.Version);
        }
    }
}
