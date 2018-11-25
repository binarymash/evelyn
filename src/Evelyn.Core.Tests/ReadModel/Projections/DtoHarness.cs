namespace Evelyn.Core.Tests.ReadModel
{
    using AutoFixture;
    using Evelyn.Core.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using FluentAssertions;
    using Newtonsoft.Json;

    public abstract class DtoHarness<TDto>
        where TDto : DtoRoot
    {
        private readonly JsonSerializerSettings _deserializeWithPrivateSetters;

        public DtoHarness()
        {
            DataFixture = new Fixture();

            _deserializeWithPrivateSetters = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };
        }

        protected Fixture DataFixture { get; }

        protected void AssertSerializationOf(TDto dto)
        {
            var serializedDto = JsonConvert.SerializeObject(dto);
            var deserializedDto = JsonConvert.DeserializeObject<TDto>(serializedDto, _deserializeWithPrivateSetters);
            deserializedDto.Should().BeEquivalentTo(dto);
        }
    }
}
