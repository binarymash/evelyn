namespace Evelyn.Core.Tests.ReadModel.EnvironmentDetails.ProjectEvents
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class EnvironmentAddedSpecs
    {
        private readonly Fixture _fixture;
        private readonly IProjectionStore<EnvironmentDetailsDto> _projectionStore;
        private readonly CancellationToken _stoppingToken;
        private ProjectEvents.EnvironmentAdded @event;
        private ProjectionBuilder _projectionBuilder;
        private EnvironmentDetailsDto _updatedProjection;
        private Exception _thrownException;
        private Guid _projectId;
        private string _key;
        private EnvironmentDetailsDto _originalProjection;

        public EnvironmentAddedSpecs()
        {
            _fixture = new Fixture();
            _projectionStore = new InMemoryProjectionStore<EnvironmentDetailsDto>();
            _stoppingToken = default;
        }

        [Fact]
        public void NoProjection()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .And(_ => GivenWeReceivedAProjectCreatedEvent())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenTheProjectionIsCreated())
                .BDDfy();
        }

        [Fact]
        public void ProjectionAlreadyExists()
        {
            this.Given(_ => GivenTheProjectionAlreadyExists())
                .And(_ => GivenWeReceivedAProjectCreatedEvent())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        private void GivenThereIsNoProjection()
        {
            _projectId = _fixture.Create<Guid>();
            _key = _fixture.Create<string>();
        }

        private void GivenTheProjectionAlreadyExists()
        {
            _originalProjection = _fixture.Create<EnvironmentDetailsDto>();
            _projectId = _originalProjection.ProjectId;
            _key = _originalProjection.Key;
        }

        private void GivenWeReceivedAProjectCreatedEvent()
        {
            @event = _fixture.Build<ProjectEvents.EnvironmentAdded>()
               .With(ea => ea.Id, _projectId)
               .With(ea => ea.Key, _key)
               .Create();
        }

        private async Task WhenTheEventIsHandled()
        {
            try
            {
                if (_originalProjection != null)
                {
                    await _projectionStore.Create(EnvironmentDetailsDto.StoreKey(_originalProjection.ProjectId, _originalProjection.Key), _originalProjection);
                }

                _projectionBuilder = new ProjectionBuilder(_projectionStore);
                await _projectionBuilder.Handle(@event, _stoppingToken);
                _updatedProjection = await _projectionStore.Get(EnvironmentDetailsDto.StoreKey(@event.Id, @event.Key));
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        private void ThenTheProjectionIsCreated()
        {
            _updatedProjection.ProjectId.Should().Be(@event.Id);
            _updatedProjection.Created.Should().Be(@event.OccurredAt);
            _updatedProjection.CreatedBy.Should().Be(@event.UserId);

            _updatedProjection.LastModified.Should().Be(@event.OccurredAt);
            _updatedProjection.LastModifiedBy.Should().Be(@event.UserId);
            _updatedProjection.Version.Should().Be(0);

            _updatedProjection.Key.Should().Be(@event.Key);
            _updatedProjection.Name.Should().Be(@event.Name);
        }

        private void ThenAnExceptionIsThrown()
        {
            _thrownException.Should().NotBeNull();
        }
    }
}
