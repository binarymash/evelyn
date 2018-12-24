namespace Evelyn.Core.Tests.ReadModel
{
    using AutoFixture;
    using Evelyn.Core.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using FluentAssertions;
    using Newtonsoft.Json;

    public abstract class ProjectionHarness<TProjection>
        where TProjection : Projection
    {
        private readonly JsonSerializerSettings _deserializeWithPrivateSetters;

        public ProjectionHarness()
        {
            DataFixture = new Fixture();

            _deserializeWithPrivateSetters = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };
        }

        protected Fixture DataFixture { get; }

        protected void AssertSerializationOf(TProjection dto)
        {
            var serializedDto = JsonConvert.SerializeObject(dto);
            var deserializedDto = JsonConvert.DeserializeObject<TProjection>(serializedDto, _deserializeWithPrivateSetters);
            deserializedDto.Should().BeEquivalentTo(dto);
        }
    }
}
