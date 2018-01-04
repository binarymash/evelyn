namespace Evelyn.Core.Tests.ReadModel.EnvironmentDetails
{
    using AutoFixture;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Xunit;

    public class EnvironmentDetailsDtoSpecs : DtoSpecs<EnvironmentDetailsDto>
    {
        [Fact]
        public void Serialization()
        {
            var environmentDetailsDto = DataFixture.Create<EnvironmentDetailsDto>();
            AssertSerializationOf(environmentDetailsDto);
        }
    }
}
