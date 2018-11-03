namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EnvironmentState;
    using Evelyn.Core.WriteModel;

    public abstract class EventSpecs<TEvent> : EventSpecs<EnvironmentStateDto, ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        public EventSpecs()
        {
            ProjectionBuilder = new ProjectionBuilder(ProjectionStore);
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
            OriginalProjection = DataFixture.Create<EnvironmentStateDto>();
            ProjectId = DataFixture.Create<Guid>();
            EnvironmentKey = DataFixture.Create<string>();
        }

        protected void GivenTheProjectionHasOtherToggleStates()
        {
            OriginalProjection.AddToggleState(
                DataFixture.Create<string>(),
                DataFixture.Create<string>(),
                DataFixture.Create<int>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>());
        }

        protected void GivenOurToggleStateIsOnTheProjection()
        {
            ToggleKey = DataFixture.Create<string>();

            OriginalProjection.AddToggleState(
                ToggleKey,
                DataFixture.Create<string>(),
                DataFixture.Create<int>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>());
        }
    }
}
