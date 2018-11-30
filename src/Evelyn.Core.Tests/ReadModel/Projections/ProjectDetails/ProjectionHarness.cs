namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Evelyn.Core.WriteModel;

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
    }
}
