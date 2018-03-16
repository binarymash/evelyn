namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using AutoFixture;

    public abstract class ProjectionBuilderSpecs
    {
        protected ProjectionBuilderSpecs()
        {
            DataFixture = new Fixture();
            StubbedRepository = new StubbedRepository();
        }

        protected Fixture DataFixture { get; }

        protected StubbedRepository StubbedRepository { get; set; }

        protected Exception ThrownException { get; set; }
    }
}
