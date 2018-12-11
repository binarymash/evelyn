namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects
{
    using System;
    using AutoFixture;
    using Evelyn.Core.WriteModel;
    using NSubstitute;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public abstract class ProjectionBuilderHarness<TEvent> : ProjectionBuilderHarness<Projections.AccountProjects.Projection, Projections.AccountProjects.ProjectionBuilder, TEvent>
        where TEvent : Event
    {
        protected ProjectionBuilderHarness()
        {
            ProjectionBuilder = new Projections.AccountProjects.ProjectionBuilder(ProjectionStore);
        }

        protected Guid AccountId { get; private set; }

        protected Guid ProjectId { get; private set; }

        protected void GivenThereIsNoProjection()
        {
            AccountId = DataFixture.Create<Guid>();
        }

        protected void GivenTheProjectionExists()
        {
            OriginalProjection = DataFixture.Create<Core.ReadModel.Projections.AccountProjects.Projection>();
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
                DataFixture.Create<Projections.EventAudit>(),
                ProjectId,
                DataFixture.Create<string>());
        }

        protected void GivenAnotherProjectIsOnTheAccount()
        {
            OriginalProjection.Account.AddProject(
                DataFixture.Create<Projections.EventAudit>(),
                DataFixture.Create<Guid>(),
                DataFixture.Create<string>());
        }

        protected void ThenTheProjectionIsCreated()
        {
            ProjectionStore.Received().Create(Core.ReadModel.Projections.AccountProjects.Projection.StoreKey(AccountId), UpdatedProjection);
        }
    }
}
