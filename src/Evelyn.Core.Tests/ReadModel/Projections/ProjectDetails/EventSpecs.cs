namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using NSubstitute;

    public abstract class EventSpecs : EventSpecs<ProjectDetailsDto>
    {
        public EventSpecs()
        {
            ProjectionBuilder = new ProjectionBuilder(ProjectionStore);
        }

        protected ProjectionBuilder ProjectionBuilder { get; private set; }

        protected Guid ProjectId { get; private set; }

        protected void GivenThereIsNoProjection()
        {
            ProjectId = DataFixture.Create<Guid>();
        }

        protected void GivenTheProjectionExists()
        {
            OriginalProjection = DataFixture.Create<ProjectDetailsDto>();
            ProjectId = OriginalProjection.Id;
        }

        protected void GivenThereAreEnvironmentsOnTheProjection()
        {
            OriginalProjection.AddEnvironment(
                DataFixture.Create<string>(),
                DataFixture.Create<string>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<int>(),
                DataFixture.Create<string>());
        }

        protected void GivenThereAreTogglesOnTheProjection()
        {
            OriginalProjection.AddToggle(
                DataFixture.Create<string>(),
                DataFixture.Create<string>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>(),
                DataFixture.Create<int>());
        }

        protected void ThenTheProjectionIsCreated()
        {
            ProjectionStore.Received().Create(ProjectDetailsDto.StoreKey(ProjectId), UpdatedProjection);
        }
    }
}
