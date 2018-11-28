namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel;
    using NSubstitute;

    public abstract class ProjectionHarness<TEvent> : ProjectionsHarness<AccountProjectsDto, ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        protected ProjectionHarness()
        {
            ProjectionBuilder = new ProjectionBuilder(ProjectionStore);
        }

        protected Guid AccountId { get; private set; }

        protected Guid ProjectId { get; private set; }

        protected void GivenThereIsNoProjection()
        {
            AccountId = DataFixture.Create<Guid>();
        }

        protected void GivenTheProjectionExists()
        {
            OriginalProjection = DataFixture.Create<AccountProjectsDto>();
            AccountId = OriginalProjection.AccountId;
        }

        protected void GivenOurProjectIsNotOnTheProjection()
        {
            ProjectId = DataFixture.Create<Guid>();
        }

        protected void GivenOurProjectIsOnTheProjection()
        {
            ProjectId = DataFixture.Create<Guid>();
            OriginalProjection.AddProject(
                ProjectId,
                DataFixture.Create<string>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>(),
                DataFixture.Create<long>());
        }

        protected void GivenAnotherProjectIsOnTheProjection()
        {
            OriginalProjection.AddProject(
                DataFixture.Create<Guid>(),
                DataFixture.Create<string>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>(),
                DataFixture.Create<long>());
        }

        protected void ThenTheProjectionIsCreated()
        {
            ProjectionStore.Received().Create(AccountProjectsDto.StoreKey(AccountId), UpdatedProjection);
        }
    }
}
