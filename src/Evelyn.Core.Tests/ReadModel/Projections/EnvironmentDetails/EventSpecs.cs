namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
    using Evelyn.Core.WriteModel;

    public abstract class EventSpecs<TEvent> : EventSpecs<EnvironmentDetailsDto, ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        protected EventSpecs()
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
            OriginalProjection = DataFixture.Create<EnvironmentDetailsDto>();
            ProjectId = OriginalProjection.ProjectId;
            EnvironmentKey = OriginalProjection.Key;
        }
    }
}
