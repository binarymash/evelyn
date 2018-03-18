namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using AutoFixture;
    using Core.ReadModel;
    using CQRSlite.Domain;
    using FluentAssertions;
    using NSubstitute;

    public abstract class ProjectionBuilderSpecs<TProjectionBuilder, TDto>
    {
        protected ProjectionBuilderSpecs()
        {
            DataFixture = new Fixture();
            SubstituteRepository = Substitute.For<IRepository>();
        }

        protected TProjectionBuilder Builder { get; set; }

        protected TDto Dto { get; set; }

        protected Fixture DataFixture { get; }

        protected IRepository SubstituteRepository { get; }

        protected Exception ThrownException { get; set; }

        protected void ThenAFailedToBuildProjectionExceptionIsThrown()
        {
            ThrownException.Should().BeOfType<FailedToBuildProjectionException>();
        }
    }
}
