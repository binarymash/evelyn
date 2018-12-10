namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
    using Evelyn.Core.WriteModel;

    public abstract class ProjectionBuilderHarness<TEvent> : ProjectionBuilderHarness<Projection, ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        protected ProjectionBuilderHarness()
        {
            ProjectionBuilder = new ProjectionBuilder(ProjectionStore);
        }

        protected Guid ProjectId { get; private set; }

        protected string EnvironmentKey { get; private set; }

        protected void GivenThereIsNoProjection()
        {
            ProjectId = DataFixture.Create<Guid>();
            EnvironmentKey = DataFixture.Create<string>();
        }

        protected void GivenTheProjectionExists()
        {
            OriginalProjection = DataFixture.Create<Projection>();
            ProjectId = OriginalProjection.Environment.ProjectId;
            EnvironmentKey = OriginalProjection.Environment.Key;
        }
    }
}
