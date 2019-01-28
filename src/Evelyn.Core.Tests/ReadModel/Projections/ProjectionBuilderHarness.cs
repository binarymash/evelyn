namespace Evelyn.Core.Tests.ReadModel.Projections
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.WriteModel;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;

    public abstract class ProjectionBuilderHarness<TProjection, TProjectionBuilder, TEvent>
        where TProjection : Projection
        where TProjectionBuilder : ProjectionBuilder<TProjection>
        where TEvent : Event
    {
        private readonly JsonSerializerSettings _deserializeWithPrivateSetters;

        protected ProjectionBuilderHarness()
        {
            DataFixture = new Fixture();
            StoppingToken = default;
            ProjectionStore = Substitute.For<IProjectionStore<TProjection>>();

            _deserializeWithPrivateSetters = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };

            ProjectionStore.Get(Arg.Any<string>())
                .Returns(ps => CopyOf(OriginalProjection));

            ProjectionStore.WhenForAnyArgs(ps => ps.Create(Arg.Any<string>(), Arg.Any<TProjection>()))
                .Do(ci => UpdatedProjection = ci.ArgAt<TProjection>(1));

            ProjectionStore.WhenForAnyArgs(ps => ps.Update(Arg.Any<string>(), Arg.Any<TProjection>()))
                .Do(ci => UpdatedProjection = ci.ArgAt<TProjection>(1));

            StreamPosition = long.MinValue;
        }

        protected Fixture DataFixture { get; }

        protected CancellationToken StoppingToken { get; }

        protected IProjectionStore<TProjection> ProjectionStore { get; set; }

        protected TProjectionBuilder ProjectionBuilder { get; set; }

        protected TProjection OriginalProjection { get; set; }

        protected TEvent Event { get; set; }

        protected long StreamPosition { get; set; }

        protected TProjection UpdatedProjection { get; set; }

        protected Exception ThrownException { get; set; }

        protected DateTimeOffset TimeBeforeHandled { get; private set; }

        protected DateTimeOffset TimeAfterHandled { get; private set; }

        protected async Task WhenTheEventIsHandled()
        {
            try
            {
                if (StreamPosition == long.MinValue)
                {
                    StreamPosition = OriginalProjection?.Audit?.StreamPosition + 1 ?? 1;
                }

                TimeBeforeHandled = DateTimeOffset.UtcNow;
                await HandleEventImplementation();
                TimeAfterHandled = DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        protected void ThenAnExceptionIsThrown()
        {
            ThrownException.Should().NotBeNull();
        }

        protected async Task ThenTheStoredProjectionIsUnchanged()
        {
            await ProjectionStore.DidNotReceiveWithAnyArgs().Create(Arg.Any<string>(), Arg.Any<TProjection>());
            await ProjectionStore.DidNotReceiveWithAnyArgs().Update(Arg.Any<string>(), Arg.Any<TProjection>());
            await ProjectionStore.DidNotReceiveWithAnyArgs().Delete(Arg.Any<string>());
        }

        protected void ThenNoProjectionIsCreated()
        {
            UpdatedProjection.Should().BeNull();
        }

        protected void ThenTheProjectionAuditIsSet()
        {
            UpdatedProjection.Audit.Generated.Should().BeOnOrAfter(TimeBeforeHandled).And.BeOnOrBefore(TimeAfterHandled);
            UpdatedProjection.Audit.StreamPosition.Should().Be(StreamPosition);
        }

        protected void AssertSerializationOf(TProjection projection)
        {
            var serializedProjection = JsonConvert.SerializeObject(projection);
            var deserializedProjection = JsonConvert.DeserializeObject<TProjection>(serializedProjection, _deserializeWithPrivateSetters);
            deserializedProjection.Should().BeEquivalentTo(projection);
        }

        protected abstract Task HandleEventImplementation();

        private TProjection CopyOf(TProjection original)
        {
            var serializedDto = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<TProjection>(serializedDto, _deserializeWithPrivateSetters);
        }
    }
}
