namespace Evelyn.Core.Tests.ReadModel.AccountProjects.AccountEvents
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using FluentAssertions;
    using NSubstitute;
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
        private IProjectionStore<AccountProjectsDto> _projectionStore;
        private Exception _exceptionFromStore;
        private CancellationToken _stoppingToken;
        private Guid _accountId;
        private AccountEvents.ProjectDeleted @event;
        private Guid _projectId;

        public ProjectDeletedSpecs()
        {
            _fixture = new Fixture();
            _projectionStore = new InMemoryProjectionStore<AccountProjectsDto>();
            _stoppingToken = default;

            _accountId = Guid.NewGuid();
        }

        [Fact]
        public void NoProjection()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleTheProjectDeletedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ProjectDoesntExist()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectWeAreDeletingIsNotOnTheProjection())
                .When(_ => WhenWeHandleTheProjectDeletedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .And(_ => ThenTheStoredProjectionIsUnchanged())
                .BDDfy();
        }

        [Fact]
        public void ProjectExists()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectWeAreDeletingIsOnTheProjection())
                .When(_ => WhenWeHandleTheProjectDeletedEvent())
                .Then(_ => ThenTheProjectionHasBeenUpdated())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByProjectionStoreWhenSaving()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectWeAreDeletingIsOnTheProjection())
                .And(_ => GivenTheProjectionStoreWillThrowWhenSaving())
                .When(_ => WhenWeHandleTheProjectDeletedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        private void GivenThereIsNoProjection()
        {
            _originalProjection = null;
        }

        private void GivenTheProjectionExists()
        {
            _originalProjection = AccountProjectsDto.Create(
                _accountId,
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());
        }

        private void GivenTheProjectWeAreDeletingIsNotOnTheProjection()
        {
            _projectId = _fixture.Create<Guid>();
        }

        private void GivenTheProjectWeAreDeletingIsOnTheProjection()
        {
            _projectId = _fixture.Create<Guid>();

            _originalProjection.AddProject(
                _projectId,
                _fixture.Create<string>(),
                1,
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());
        }

        private void GivenTheProjectionStoreWillThrowWhenSaving()
        {
            _projectionStore = Substitute.For<IProjectionStore<AccountProjectsDto>>();

            _exceptionFromStore = new Exception();
            _projectionStore.Delete(Arg.Any<string>())
                .Returns(ps => throw _exceptionFromStore);
        }

        private async Task WhenWeHandleTheProjectDeletedEvent()
        {
            @event = _fixture.Build<AccountEvents.ProjectDeleted>()
                .With(e => e.Id, _accountId)
                .With(e => e.ProjectId, _projectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private async Task WhenTheEventIsHandled()
        {
            if (_originalProjection != null)
            {
                await _projectionStore.Create(AccountProjectsDto.StoreKey(_originalProjection.AccountId), _originalProjection);
            }

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
            catch (ProjectionNotFoundException)
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
