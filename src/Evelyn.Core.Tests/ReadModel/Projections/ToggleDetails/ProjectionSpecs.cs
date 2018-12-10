namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails
{
    using AutoFixture;
    using Core.ReadModel.Projections.ToggleDetails;
    using Xunit;

    public class ProjectionSpecs : ProjectionHarness<Projection>
    {
        [Fact]
        public void Serialization()
        {
            var dto = DataFixture.Create<Projection>();
            AssertSerializationOf(dto);
        }
    }
}
