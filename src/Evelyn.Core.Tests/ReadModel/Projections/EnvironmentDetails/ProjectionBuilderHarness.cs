namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.WriteModel;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public abstract class ProjectionBuilderHarness<TEvent> : ProjectionBuilderHarness<Projections.EnvironmentDetails.Projection, Projections.EnvironmentDetails.ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        protected ProjectionBuilderHarness()
        {
            ProjectionBuilder = new Projections.EnvironmentDetails.ProjectionBuilder(ProjectionStore);
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
            OriginalProjection = DataFixture.Create<Projections.EnvironmentDetails.Projection>();
            ProjectId = OriginalProjection.Environment.ProjectId;
            EnvironmentKey = OriginalProjection.Environment.Key;
        }
    }
}
