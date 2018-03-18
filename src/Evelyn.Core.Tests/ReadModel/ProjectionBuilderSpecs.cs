namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using AutoFixture;
    using CQRSlite.Domain;
    using NSubstitute;

    public abstract class ProjectionBuilderSpecs
    {
        protected ProjectionBuilderSpecs()
        {
            DataFixture = new Fixture();
            SubstituteRepository = Substitute.For<IRepository>();
        }

        protected Fixture DataFixture { get; }

        protected IRepository SubstituteRepository { get; }

        protected Exception ThrownException { get; set; }
    }
}
