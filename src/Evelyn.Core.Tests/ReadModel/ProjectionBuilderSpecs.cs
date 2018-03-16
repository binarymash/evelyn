namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using AutoFixture;
    using CQRSlite.Events;

    public abstract class ProjectionBuilderSpecs
    {
        private readonly IEventPublisher _publisher;

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
