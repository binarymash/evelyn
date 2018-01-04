namespace Evelyn.Core.Tests.ReadModel
{
    using AutoFixture;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using FluentAssertions;
    using Newtonsoft.Json;

    public class DtoSpecs<T>
    {
        private readonly JsonSerializerSettings _deserializeWithPrivateSetters;

        public DtoSpecs()
        {
            DataFixture = new Fixture();

            _deserializeWithPrivateSetters = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };
        }

        protected Fixture DataFixture { get; }

        protected void AssertSerializationOf(T dto)
        {
            var serializedDto = JsonConvert.SerializeObject(dto);
            var deserializedDto = JsonConvert.DeserializeObject<T>(serializedDto, _deserializeWithPrivateSetters);
            deserializedDto.Should().BeEquivalentTo(dto);
        }
    }
}
