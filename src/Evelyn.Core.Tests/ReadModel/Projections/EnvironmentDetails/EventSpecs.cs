namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
    using FluentAssertions;
    using NSubstitute;

    public abstract class EventSpecs
    {
        protected EventSpecs()
        {
            DataFixture = new Fixture();
            ProjectionStore = new InMemoryProjectionStore<EnvironmentDetailsDto>();
            StoppingToken = default;
        }

        protected Fixture DataFixture { get; }

        protected ProjectionBuilder ProjectionBuilder { get; private set; }

        protected CancellationToken StoppingToken { get; }

        protected IProjectionStore<EnvironmentDetailsDto> ProjectionStore { get; private set; }

        protected Guid ProjectId { get; private set; }

        protected string EnvironmentKey { get; private set; }

        protected Exception ThrownException { get; private set; }

        protected EnvironmentDetailsDto OriginalProjection { get; private set; }

        protected EnvironmentDetailsDto UpdatedProjection { get; private set; }

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

        protected void GivenTheProjectionStoreWillThrowWhenCreating()
        {
            ProjectionStore = Substitute.For<IProjectionStore<EnvironmentDetailsDto>>();

            ProjectionStore.Create(Arg.Any<string>(), Arg.Any<EnvironmentDetailsDto>())
                .Returns(ps => throw new Exception());
        }

        protected void GivenTheProjectionStoreWillThrowWhenUpdating()
        {
            ProjectionStore = Substitute.For<IProjectionStore<EnvironmentDetailsDto>>();

            ProjectionStore.Update(Arg.Any<string>(), Arg.Any<EnvironmentDetailsDto>())
                .Returns(ps => throw new Exception());
        }

        protected void GivenTheProjectionStoreWillThrowWhenDeleting()
        {
            ProjectionStore = Substitute.For<IProjectionStore<EnvironmentDetailsDto>>();

            ProjectionStore.Delete(Arg.Any<string>())
                .Returns(ps => throw new Exception());
        }

        protected async Task WhenTheEventIsHandled()
        {
            if (OriginalProjection != null)
            {
                await ProjectionStore.Create(EnvironmentDetailsDto.StoreKey(OriginalProjection.ProjectId, OriginalProjection.Key), OriginalProjection);
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
                UpdatedProjection = await ProjectionStore.Get(EnvironmentDetailsDto.StoreKey(ProjectId, EnvironmentKey));
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
                var currentProjection = await ProjectionStore.Get(EnvironmentDetailsDto.StoreKey(ProjectId, EnvironmentKey));
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
