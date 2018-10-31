namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
    using FluentAssertions;
    using NSubstitute;

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

        protected void ThenAnExceptionIsThrown()
        {
            ThrownException.Should().NotBeNull();
        }
    }
}
