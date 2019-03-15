namespace Evelyn.Core.Tests.ReadModel.Projections.ClientEnvironmentState
{
    using System;
    using AutoFixture;
    using Evelyn.Core.WriteModel;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public abstract class ProjectionBuilderHarness<TEvent> : ProjectionBuilderHarness<Projections.ClientEnvironmentState.Projection, Projections.ClientEnvironmentState.ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        public ProjectionBuilderHarness()
        {
            ProjectionBuilder = new Projections.ClientEnvironmentState.ProjectionBuilder(ProjectionStore);
        }

        protected Guid ProjectId { get; set; }

        protected string EnvironmentKey { get; set; }

        protected string ToggleKey { get; private set; }

        protected void GivenThereIsNoProjection()
        {
            ProjectId = DataFixture.Create<Guid>();
            EnvironmentKey = DataFixture.Create<string>();
        }

        protected void GivenTheProjectionExists()
        {
            OriginalProjection = DataFixture.Create<Projections.ClientEnvironmentState.Projection>();
            ProjectId = DataFixture.Create<Guid>();
            EnvironmentKey = DataFixture.Create<string>();
        }

        protected void GivenTheProjectionHasOtherToggleStates()
        {
            OriginalProjection.EnvironmentState.AddToggleState(
                DataFixture.Create<Projections.EventAudit>(),
                DataFixture.Create<string>(),
                DataFixture.Create<string>());
        }

        protected void GivenOurToggleStateIsOnTheProjection()
        {
            ToggleKey = DataFixture.Create<string>();

            OriginalProjection.EnvironmentState.AddToggleState(
                DataFixture.Create<Projections.EventAudit>(),
                ToggleKey,
                DataFixture.Create<string>());
        }
    }
}
