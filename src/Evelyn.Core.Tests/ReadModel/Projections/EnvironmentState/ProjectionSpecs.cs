namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState
{
    using AutoFixture;
    using Xunit;
    using Projections = Core.ReadModel.Projections;

    public class ProjectionSpecs : ProjectionHarness<Projections.EnvironmentState.Projection>
    {
        [Fact]
        public void Serialization()
        {
            var dto = DataFixture.Create<Projections.EnvironmentState.Projection>();
            AssertSerializationOf(dto);
        }
    }
}
