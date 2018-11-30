namespace Evelyn.Core.Tests.ReadModel.Projections.EventHandlerState
{
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EventHandlerState;
    using Xunit;

    public class EventHandlerStateSpecs : DtoHarness<EventHandlerStateDto>
    {
        [Fact]
        public void Serialization()
        {
            var dto = DataFixture.Create<EventHandlerStateDto>();
            AssertSerializationOf(dto);
        }
    }
}
