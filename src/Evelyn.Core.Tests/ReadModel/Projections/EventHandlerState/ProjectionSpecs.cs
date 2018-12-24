namespace Evelyn.Core.Tests.ReadModel.Projections.EventHandlerState
{
    using AutoFixture;
    using Xunit;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public class ProjectionSpecs : ProjectionHarness<Projections.EventHandlerState.Projection>
    {
        [Fact]
        public void Serialization()
        {
            var projection = DataFixture.Create<Projections.EventHandlerState.Projection>();
            AssertSerializationOf(projection);
        }
    }
}
