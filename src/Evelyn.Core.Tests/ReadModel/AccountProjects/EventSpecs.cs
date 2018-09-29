namespace Evelyn.Core.Tests.ReadModel.AccountProjects
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using FluentAssertions;
    using NSubstitute;

    public abstract class EventSpecs
    {
        protected EventSpecs()
        {
            DataFixture = new Fixture();
            ProjectionStore = new InMemoryProjectionStore<AccountProjectsDto>();
            StoppingToken = default;
        }

        protected Fixture DataFixture { get; }

        protected ProjectionBuilder ProjectionBuilder { get; private set; }

        protected CancellationToken StoppingToken { get; }

        protected IProjectionStore<AccountProjectsDto> ProjectionStore { get; private set; }

        protected Guid AccountId { get; private set; }

        protected Guid ProjectId { get; private set; }

        protected Exception ThrownException { get; private set; }

        protected AccountProjectsDto OriginalProjection { get; private set; }

        protected AccountProjectsDto UpdatedProjection { get; private set; }

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
                DataFixture.Create<int>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>());
        }

        protected void GivenAnotherProjectIsOnTheProjection()
        {
            OriginalProjection.AddProject(
                DataFixture.Create<Guid>(),
                DataFixture.Create<string>(),
                DataFixture.Create<int>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>());
        }

        protected void GivenTheProjectionStoreWillThrowWhenCreating()
        {
            ProjectionStore = Substitute.For<IProjectionStore<AccountProjectsDto>>();

            ProjectionStore.Create(Arg.Any<string>(), Arg.Any<AccountProjectsDto>())
                .Returns(ps => throw new Exception());
        }

        protected void GivenTheProjectionStoreWillThrowWhenUpdating()
        {
            ProjectionStore = Substitute.For<IProjectionStore<AccountProjectsDto>>();

            ProjectionStore.Update(Arg.Any<string>(), Arg.Any<AccountProjectsDto>())
                .Returns(ps => throw new Exception());
        }

        protected void GivenTheProjectionStoreWillThrowWhenDeleting()
        {
            ProjectionStore = Substitute.For<IProjectionStore<AccountProjectsDto>>();

            ProjectionStore.Delete(Arg.Any<string>())
                .Returns(ps => throw new Exception());
        }

        protected async Task WhenTheEventIsHandled()
        {
            if (OriginalProjection != null)
            {
                await ProjectionStore.Create(AccountProjectsDto.StoreKey(OriginalProjection.AccountId), OriginalProjection);
            }

            ProjectionBuilder = new ProjectionBuilder(ProjectionStore);
            try
            {
                await HandleEventImplementation();
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }

            try
            {
                UpdatedProjection = await ProjectionStore.Get(AccountProjectsDto.StoreKey(AccountId));
            }
            catch
            {
                UpdatedProjection = null;
            }
        }

        protected void ThenAnExceptionIsThrown()
        {
            ThrownException.Should().NotBeNull();
        }

        protected async Task ThenTheStoredProjectionIsUnchanged()
        {
            try
            {
                var currentProjection = await ProjectionStore.Get(AccountProjectsDto.StoreKey(AccountId));
                currentProjection.Should().BeEquivalentTo(OriginalProjection);
            }
            catch (ProjectionNotFoundException)
            {
                OriginalProjection.Should().BeNull();
            }
        }

        protected abstract Task HandleEventImplementation();
    }
}
