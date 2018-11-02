namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ToggleDetails;
    using NSubstitute;

    public abstract class EventSpecs : EventSpecs<ToggleDetailsDto>
    {
        public EventSpecs()
        {
            ProjectionBuilder = new ProjectionBuilder(ProjectionStore);
        }

        protected ProjectionBuilder ProjectionBuilder { get; private set; }

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

        protected virtual void ThenTheProjectionIsCreated()
        {
            ProjectionStore.Received().Create(ToggleDetailsDto.StoreKey(ProjectId, ToggleKey), UpdatedProjection);
        }
    }
}
