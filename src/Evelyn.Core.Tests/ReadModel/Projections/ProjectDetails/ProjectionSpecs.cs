namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails
{
    using AutoFixture;
    using Core.ReadModel.Projections.ProjectDetails;
    using Xunit;

    public class ProjectionSpecs : ProjectionHarness<Projection>
    {
        [Fact]
        public void Serialization()
        {
            var projection = DataFixture.Create<Projection>();
            AssertSerializationOf(projection);
        }
    }
}
