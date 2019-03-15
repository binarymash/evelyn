namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleState
{
    using System;
    using AutoFixture;
    using Evelyn.Core.WriteModel;
    using NSubstitute;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public abstract class ProjectionBuilderHarness<TEvent> : ProjectionBuilderHarness<Projections.ToggleState.Projection, Projections.ToggleState.ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        public ProjectionBuilderHarness()
        {
            ProjectionBuilder = new Projections.ToggleState.ProjectionBuilder(ProjectionStore);
        }

        protected Guid ProjectId { get; set; }

        protected string ToggleKey { get; private set; }

        protected string EnvironmentKey { get; set; }

        protected void GivenThereAreNoProjections()
        {
            ProjectId = DataFixture.Create<Guid>();
            ToggleKey = DataFixture.Create<string>();

            ProjectionStore.WhenForAnyArgs(ps => ps.Get(Arg.Any<string>()))
                .Do(ci => throw new Evelyn.Core.ReadModel.ProjectionNotFoundException());
        }

        protected void GivenTheProjectionExists()
        {
            OriginalProjection = DataFixture.Create<Projections.ToggleState.Projection>();
            ProjectId = DataFixture.Create<Guid>();
            ToggleKey = DataFixture.Create<string>();
        }

        protected void GivenTheProjectionHasStateForAnotherEnvironment()
        {
            OriginalProjection.ToggleState.AddEnvironmentState(
                DataFixture.Create<Projections.EventAudit>(),
                DataFixture.Create<string>(),
                DataFixture.Create<string>());
        }

        protected void GivenTheProjectionHasStateForOurEnvironment()
        {
            EnvironmentKey = DataFixture.Create<string>();

            OriginalProjection.ToggleState.AddEnvironmentState(
                DataFixture.Create<Projections.EventAudit>(),
                EnvironmentKey,
                DataFixture.Create<string>());
        }

        protected void GivenTheProjectionStreamVersionIsTheSameAsTheNextEvent()
        {
            StreamPosition = OriginalProjection.Audit.StreamPosition;
        }
    }
}
