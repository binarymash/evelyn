namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects
{
    using System;
    using AutoFixture;
    using Xunit;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public class ProjectionSpecs : ProjectionHarness<Projections.AccountProjects.Projection>
    {
        [Fact]
        public void Serialization()
        {
            var projection = Projections.AccountProjects.Projection.Create(
                DataFixture.Create<Projections.EventAudit>(),
                DataFixture.Create<Projections.AccountProjects.Model.Account>());

            projection.Account.AddProject(
                DataFixture.Create<Projections.EventAudit>(),
                DataFixture.Create<Guid>(),
                DataFixture.Create<string>());

            AssertSerializationOf(projection);
        }
    }
}
