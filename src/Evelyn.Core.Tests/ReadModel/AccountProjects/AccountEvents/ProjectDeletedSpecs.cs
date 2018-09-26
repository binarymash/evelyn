namespace Evelyn.Core.Tests.ReadModel.AccountProjects.AccountEvents
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvents = Evelyn.Core.WriteModel.Account.Events;

    public class ProjectDeletedSpecs
    {
        private AccountProjectsDto _originalProjection;
        private ProjectionBuilder _projectionBuilder;
        private AccountProjectsDto _updatedProjection;
        private Exception _thrownException;
        private Fixture _fixture;
        private InMemoryProjectionStore<AccountProjectsDto> _projectionStore;
        private CancellationToken _stoppingToken;
        private Guid _accountId;
        private AccountEvents.ProjectDeleted @event;

        public ProjectDeletedSpecs()
        {
            _fixture = new Fixture();
            _projectionStore = new InMemoryProjectionStore<AccountProjectsDto>();
            _stoppingToken = default;

            _accountId = Guid.NewGuid();
            @event = _fixture.Build<AccountEvents.ProjectDeleted>()
                .With(pc => pc.Id, _accountId)
                .Create();
        }

        [Fact]
        public void NoProjection()
        {
            this.Given(_ => GivenThereIsNoProjectionForTheAccount())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenAnExceptionIsThrown())
                .And(_ => ThenTheStoredProjectionIsUnchanged())
                .BDDfy();
        }

        [Fact]
        public void ProjectDoesntExist()
        {
            this.Given(_ => GivenTheProjectWeAreDeletingIsNotOnTheProjection())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenAnExceptionIsThrown())
                .And(_ => ThenTheStoredProjectionIsUnchanged())
                .BDDfy();
        }

        [Fact]
        public void ProjectExists()
        {
            this.Given(_ => GivenTheProjectWeAreDeletingIsOnTheProjection())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenTheProjectionHasBeenUpdated())
                .BDDfy();
        }

        private void GivenThereIsNoProjectionForTheAccount()
        {
            _originalProjection = null;
        }

        private async Task GivenTheProjectWeAreDeletingIsNotOnTheProjection()
        {
            _originalProjection = AccountProjectsDto.Create(
                _accountId,
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());

            await _projectionStore.AddOrUpdate(AccountProjectsDto.StoreKey(_originalProjection.AccountId), _originalProjection);
        }

        private async Task GivenTheProjectWeAreDeletingIsOnTheProjection()
        {
            _originalProjection = AccountProjectsDto.Create(
                _accountId,
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());

            _originalProjection.AddProject(
                @event.ProjectId,
                _fixture.Create<string>(),
                1,
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());

            await _projectionStore.AddOrUpdate(AccountProjectsDto.StoreKey(_originalProjection.AccountId), _originalProjection);
        }

        private async Task WhenTheEventIsHandled()
        {
            _projectionBuilder = new ProjectionBuilder(_projectionStore);
            try
            {
                await _projectionBuilder.Handle(@event, _stoppingToken);
                _updatedProjection = await _projectionStore.Get(AccountProjectsDto.StoreKey(@event.Id));
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        private void ThenAnExceptionIsThrown()
        {
            _thrownException.Should().NotBeNull();
        }

        private async Task ThenTheStoredProjectionIsUnchanged()
        {
            try
            {
                var currentProjection = await _projectionStore.Get(AccountProjectsDto.StoreKey(_accountId));
                currentProjection.Should().BeEquivalentTo(_originalProjection);
            }
            catch (NotFoundException)
            {
                _originalProjection.Should().BeNull();
            }
        }

        private void ThenTheProjectionHasBeenUpdated()
        {
            _updatedProjection.AccountId.Should().Be(_originalProjection.AccountId);
            _updatedProjection.Created.Should().Be(_originalProjection.Created);
            _updatedProjection.CreatedBy.Should().Be(_originalProjection.CreatedBy);

            _updatedProjection.LastModified.Should().Be(@event.OccurredAt);
            _updatedProjection.LastModifiedBy.Should().Be(@event.UserId);
            _updatedProjection.Version.Should().Be(@event.Version);

            var projects = _updatedProjection.Projects.ToList();
            projects.Count.Should().Be(_originalProjection.Projects.Count() - 1);
        }
    }
}
