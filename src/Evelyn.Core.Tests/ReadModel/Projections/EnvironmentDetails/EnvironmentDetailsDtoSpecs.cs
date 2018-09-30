namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails
{
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
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
