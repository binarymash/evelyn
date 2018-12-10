namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails
{
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
    using Xunit;

    public class ProjectionSpecs : ProjectionHarness<Projection>
    {
        [Fact]
        public void Serialization()
        {
            var environmentDetailsDto = DataFixture.Create<Projection>();
            AssertSerializationOf(environmentDetailsDto);
        }
    }
}
