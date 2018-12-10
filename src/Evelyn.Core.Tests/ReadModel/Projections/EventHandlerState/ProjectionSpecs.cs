namespace Evelyn.Core.Tests.ReadModel.Projections.EventHandlerState
{
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EventHandlerState;
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
