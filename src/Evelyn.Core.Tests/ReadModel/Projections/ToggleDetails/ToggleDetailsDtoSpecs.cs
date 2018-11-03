namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails
{
    using AutoFixture;
    using Core.ReadModel.Projections.ToggleDetails;
    using Xunit;

    public class ToggleDetailsDtoSpecs : DtoHarness<ToggleDetailsDto>
    {
        [Fact]
        public void Serialization()
        {
            var dto = DataFixture.Create<ToggleDetailsDto>();
            AssertSerializationOf(dto);
        }
    }
}
