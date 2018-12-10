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

            StreamVersion = DataFixture.Create<long>();
        }

        protected Fixture DataFixture { get; }

        protected CancellationToken StoppingToken { get; }

        protected IProjectionStore<TProjection> ProjectionStore { get; set; }

        protected TProjectionBuilder ProjectionBuilder { get; set; }

        protected TProjection OriginalProjection { get; set; }

        protected TEvent Event { get; set; }

        protected long StreamVersion { get; set; }

        protected TProjection UpdatedProjection { get; set; }

        protected Exception ThrownException { get; set; }

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

        protected void ThenAnExceptionIsThrown()
        {
            ThrownException.Should().NotBeNull();
        }

        protected void ThenTheStoredProjectionIsUnchanged()
        {
            UpdatedProjection.Should().BeEquivalentTo(OriginalProjection);
        }

        protected void ThenNoProjectionIsCreated()
        {
            UpdatedProjection.Should().BeNull();
        }

        protected void ThenTheProjectionAuditIsSet()
        {
            UpdatedProjection.Audit.Created.Should().Be(Event.OccurredAt);
            UpdatedProjection.Audit.CreatedBy.Should().Be(Event.UserId);
            UpdatedProjection.Audit.Version.Should().Be(StreamVersion);
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
