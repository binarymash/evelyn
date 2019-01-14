namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleState
{
    using AutoFixture;
    using Xunit;
    using Projections = Core.ReadModel.Projections;

    public class ProjectionSpecs : ProjectionHarness<Projections.ToggleState.Projection>
    {
        [Fact]
        public void Serialization()
        {
            var dto = DataFixture.Create<Projections.ToggleState.Projection>();
            AssertSerializationOf(dto);
        }
    }
}
