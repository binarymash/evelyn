namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails
{
    using AutoFixture;
    using Xunit;
    using Projections = Core.ReadModel.Projections;

    public class ProjectionSpecs : ProjectionHarness<Projections.ToggleDetails.Projection>
    {
        [Fact]
        public void Serialization()
        {
            var dto = DataFixture.Create<Projections.ToggleDetails.Projection>();
            AssertSerializationOf(dto);
        }
    }
}
