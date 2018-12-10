namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Evelyn.Core.WriteModel;
    using NSubstitute;

    public abstract class ProjectionBuilderHarness<TEvent> : ProjectionBuilderHarness<Projection, ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        protected ProjectionBuilderHarness()
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
            OriginalProjection = DataFixture.Create<Projection>();
            AccountId = OriginalProjection.Account.AccountId;
        }

        protected void GivenOurProjectIsNotOnTheProjection()
        {
            ProjectId = DataFixture.Create<Guid>();
        }

        protected void GivenOurProjectIsOnTheAccount()
        {
            ProjectId = DataFixture.Create<Guid>();
            OriginalProjection.Account.AddProject(
                DataFixture.Create<EventAuditDto>(),
                ProjectId,
                DataFixture.Create<string>());
        }

        protected void GivenAnotherProjectIsOnTheAccount()
        {
            OriginalProjection.Account.AddProject(
                DataFixture.Create<EventAuditDto>(),
                DataFixture.Create<Guid>(),
                DataFixture.Create<string>());
        }

        protected void ThenTheProjectionIsCreated()
        {
            ProjectionStore.Received().Create(Projection.StoreKey(AccountId), UpdatedProjection);
        }
    }
}
