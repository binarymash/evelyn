namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.WriteModel;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public abstract class ProjectionBuilderHarness<TEvent> : ProjectionBuilderHarness<Projections.ToggleDetails.Projection, Projections.ToggleDetails.ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        public ProjectionBuilderHarness()
        {
            ProjectionBuilder = new Projections.ToggleDetails.ProjectionBuilder(ProjectionStore);
        }

        protected Guid ProjectId { get; private set; }

        protected string ToggleKey { get; private set; }

        protected void GivenThereIsNoProjection()
        {
            ProjectId = DataFixture.Create<Guid>();
            ToggleKey = DataFixture.Create<string>();
        }

        protected void GivenTheProjectionExists()
        {
            ProjectId = DataFixture.Create<Guid>();

            OriginalProjection = DataFixture.Create<Projections.ToggleDetails.Projection>();
            ToggleKey = OriginalProjection.Toggle.Key;
        }
    }
}
