namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.WriteModel;
    using FluentAssertions;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public abstract class ProjectionBuilderHarness<TEvent> : ProjectionBuilderHarness<Projections.ProjectDetails.Projection, Projections.ProjectDetails.ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        public ProjectionBuilderHarness()
        {
            ProjectionBuilder = new Projections.ProjectDetails.ProjectionBuilder(ProjectionStore);
        }

        protected Guid ProjectId { get; private set; }

        protected void GivenThereIsNoProjection()
        {
            ProjectId = DataFixture.Create<Guid>();
        }

        protected void GivenTheProjectionExists()
        {
            OriginalProjection = DataFixture.Create<Core.ReadModel.Projections.ProjectDetails.Projection>();
            ProjectId = OriginalProjection.Project.Id;
        }

        protected void GivenThereAreEnvironmentsOnTheProject()
        {
            OriginalProjection.Project.AddEnvironment(
                DataFixture.Create<Projections.EventAudit>(),
                DataFixture.Create<string>(),
                DataFixture.Create<string>());
        }

        protected void GivenThereAreTogglesOnTheProject()
        {
            OriginalProjection.Project.AddToggle(
                DataFixture.Create<Projections.EventAudit>(),
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
