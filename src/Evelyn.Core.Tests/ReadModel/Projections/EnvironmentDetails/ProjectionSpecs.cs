namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails
{
    using AutoFixture;
    using Xunit;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public class ProjectionSpecs : ProjectionHarness<Projections.EnvironmentDetails.Projection>
    {
        [Fact]
        public void Serialization()
        {
            var environmentDetailsDto = DataFixture.Create<Projections.EnvironmentDetails.Projection>();
            AssertSerializationOf(environmentDetailsDto);
        }
    }
}
