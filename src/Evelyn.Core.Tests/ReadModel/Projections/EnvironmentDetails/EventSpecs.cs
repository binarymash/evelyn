namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;

    public abstract class EventSpecs : EventSpecs<EnvironmentDetailsDto>
    {
        protected EventSpecs()
        {
            ProjectionBuilder = new ProjectionBuilder(ProjectionStore);
        }

        protected ProjectionBuilder ProjectionBuilder { get; private set; }

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
