namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ToggleDetails;
    using Evelyn.Core.WriteModel;

    public abstract class EventSpecs<TEvent> : EventSpecs<ToggleDetailsDto, ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        public EventSpecs()
        {
            ProjectionBuilder = new ProjectionBuilder(ProjectionStore);
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

            OriginalProjection = DataFixture.Create<ToggleDetailsDto>();
            ToggleKey = OriginalProjection.Key;
        }
    }
}
