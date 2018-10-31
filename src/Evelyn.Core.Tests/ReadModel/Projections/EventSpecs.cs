namespace Evelyn.Core.Tests.ReadModel.Projections
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;

    public abstract class EventSpecs<TDto>
        where TDto : DtoRoot
    {
        private readonly JsonSerializerSettings _deserializeWithPrivateSetters;

        protected EventSpecs()
        {
            DataFixture = new Fixture();
            StoppingToken = default;
            ProjectionStore = Substitute.For<IProjectionStore<TDto>>();

            _deserializeWithPrivateSetters = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };

            ProjectionStore.Get(Arg.Any<string>())
                .Returns(ps => CopyOf(OriginalProjection));

            ProjectionStore.WhenForAnyArgs(ps => ps.Create(Arg.Any<string>(), Arg.Any<TDto>()))
                .Do(ci => UpdatedProjection = ci.ArgAt<TDto>(1));

            ProjectionStore.WhenForAnyArgs(ps => ps.Update(Arg.Any<string>(), Arg.Any<TDto>()))
                .Do(ci => UpdatedProjection = ci.ArgAt<TDto>(1));
        }

        protected Fixture DataFixture { get; }

        protected CancellationToken StoppingToken { get; }

        protected IProjectionStore<TDto> ProjectionStore { get; set; }

        protected Exception ThrownException { get; set; }

        protected TDto OriginalProjection { get; set; }

        protected TDto UpdatedProjection { get; set; }

        protected async Task WhenTheEventIsHandled()
        {
            try
            {
                await HandleEventImplementation();
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        protected void ThenTheStoredProjectionIsUnchanged()
        {
            UpdatedProjection.Should().BeEquivalentTo(OriginalProjection);
        }

        protected void ThenNoProjectionIsCreated()
        {
            UpdatedProjection.Should().BeNull();
        }

        protected abstract Task HandleEventImplementation();

        private TDto CopyOf(TDto original)
        {
            var serializedDto = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<TDto>(serializedDto, _deserializeWithPrivateSetters);
        }
    }
}
