namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Xunit;

    public class ProjectionSpecs : ProjectionHarness<Projection>
    {
        [Fact]
        public void Serialization()
        {
            var projection = Projection.Create(
                DataFixture.Create<EventAuditDto>(),
                DataFixture.Create<Evelyn.Core.ReadModel.Projections.AccountProjects.Model.Account>());

            projection.Account.AddProject(
                DataFixture.Create<EventAuditDto>(),
                DataFixture.Create<Guid>(),
                DataFixture.Create<string>());

            AssertSerializationOf(projection);
        }
    }
}
