namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState
{
    using AutoFixture;
    using Core.ReadModel.Projections.EnvironmentState;
    using Xunit;

    public class EnvironmentStateDtoSpecs : DtoHarness<EnvironmentStateDto>
    {
        [Fact]
        public void Serialization()
        {
            var dto = DataFixture.Create<EnvironmentStateDto>();
            AssertSerializationOf(dto);
        }
    }
}
