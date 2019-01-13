namespace Evelyn.Core.Tests.ReadModel.Projections.ClientEnvironmentState
{
    using AutoFixture;
    using Xunit;
    using Projections = Core.ReadModel.Projections;

    public class ProjectionSpecs : ProjectionHarness<Projections.ClientEnvironmentState.Projection>
    {
        [Fact]
        public void Serialization()
        {
            var dto = DataFixture.Create<Projections.ClientEnvironmentState.Projection>();
            AssertSerializationOf(dto);
        }
    }
}
