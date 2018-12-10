namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState
{
    using AutoFixture;
    using Core.ReadModel.Projections.EnvironmentState;
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
