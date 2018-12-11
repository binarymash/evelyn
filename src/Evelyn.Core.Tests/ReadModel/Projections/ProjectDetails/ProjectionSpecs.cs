namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails
{
    using AutoFixture;
    using Xunit;
    using Projections = Core.ReadModel.Projections;

    public class ProjectionSpecs : ProjectionHarness<Projections.ProjectDetails.Projection>
    {
        [Fact]
        public void Serialization()
        {
            var projection = DataFixture.Create<Projections.ProjectDetails.Projection>();
            AssertSerializationOf(projection);
        }
    }
}
