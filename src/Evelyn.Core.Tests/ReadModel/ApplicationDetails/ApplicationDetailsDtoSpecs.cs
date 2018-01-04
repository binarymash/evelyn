namespace Evelyn.Core.Tests.ReadModel.ApplicationDetails
{
    using System.Linq;
    using AutoFixture;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using FluentAssertions;
    using Newtonsoft.Json;
    using Xunit;

    public class ApplicationDetailsDtoSpecs
    {
        private readonly Fixture _fixture;
        private readonly JsonSerializerSettings _deserializeWithPrivateSetters;

        public ApplicationDetailsDtoSpecs()
        {
            _fixture = new Fixture();
            _deserializeWithPrivateSetters = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };
        }

        [Fact]
        public void Serialization()
        {
            var applicationDetails = _fixture.Create<ApplicationDetailsDto>();
            var serializedApplicationDetails = JsonConvert.SerializeObject(applicationDetails);
            var deserializedApplicationDetails = JsonConvert.DeserializeObject<ApplicationDetailsDto>(serializedApplicationDetails, _deserializeWithPrivateSetters);
            deserializedApplicationDetails.Should().BeEquivalentTo(applicationDetails);
        }
    }
}
