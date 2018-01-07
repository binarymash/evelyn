namespace Evelyn.Core.Tests.ReadModel.ToggleDetails
{
    using AutoFixture;
    using Core.ReadModel.ToggleDetails;
    using Xunit;

    public class ToggleDetailsDtoSpecs : DtoSpecs<ToggleDetailsDto>
    {
        [Fact]
        public void Serialization()
        {
            var dto = DataFixture.Create<ToggleDetailsDto>();
            AssertSerializationOf(dto);
        }
    }
}
