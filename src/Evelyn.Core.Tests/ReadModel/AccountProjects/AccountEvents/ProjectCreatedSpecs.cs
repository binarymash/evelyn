namespace Evelyn.Core.Tests.ReadModel.AccountProjects.AccountEvents
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel.Account.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectCreatedSpecs
    {
        private Fixture _fixture;
        private IProjectionStore<AccountProjectsDto> _projectionStore;
        private Exception _exceptionFromStore;
        private ProjectCreated @event;
        private ProjectionBuilder _projectionBuilder;
        private CancellationToken _stoppingToken;
        private Guid _accountId;
        private Guid _projectId;
        private AccountProjectsDto _originalProjection;
        private Exception _thrownException;
        private AccountProjectsDto _updatedProjection;


        public ProjectCreatedSpecs()
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
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurProjectIsNotOnTheProjection())
                .And(_ => GivenAnotherProjectIsOnTheProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectionHasBeenUpdated())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByProjectionStoreWhenSaving()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurProjectIsNotOnTheProjection())
                .And(_ => GivenTheProjectionStoreWillThrowWhenSaving())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
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

        private void GivenOurProjectIsNotOnTheProjection()
        {
            _projectId = _fixture.Create<Guid>();
        }

        private void GivenAnotherProjectIsOnTheProjection()
        {
            _originalProjection.AddProject(
                _fixture.Create<Guid>(),
                _fixture.Create<string>(),
                _fixture.Create<int>(),
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());
        }

        private void GivenTheProjectionStoreWillThrowWhenSaving()
        {
            _projectionStore = Substitute.For<IProjectionStore<AccountProjectsDto>>();

            _exceptionFromStore = new Exception();
            _projectionStore.Update(Arg.Any<string>(), Arg.Any<AccountProjectsDto>())
                .Returns(ps => throw _exceptionFromStore);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            @event = _fixture.Build<ProjectCreated>()
                .With(pc => pc.Id, _accountId)
                .With(pc => pc.ProjectId, _projectId)
                .Create();

            await WhenWeHandleTheEvent();
        }

        private async Task WhenWeHandleTheEvent()
        {
            if (_originalProjection != null)
            {
                await _projectionStore.Create(AccountProjectsDto.StoreKey(_accountId), _originalProjection);
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

        private void ThenTheProjectionHasBeenUpdated()
        {
            _updatedProjection.AccountId.Should().Be(_originalProjection.AccountId);
            _updatedProjection.Created.Should().Be(_originalProjection.Created);
            _updatedProjection.CreatedBy.Should().Be(_originalProjection.CreatedBy);

            _updatedProjection.LastModified.Should().Be(@event.OccurredAt);
            _updatedProjection.LastModifiedBy.Should().Be(@event.UserId);
            _updatedProjection.Version.Should().Be(@event.Version);

            var projects = _updatedProjection.Projects.ToList();

            projects.Count.Should().Be(_originalProjection.Projects.Count() + 1);

            foreach (var originalProject in _originalProjection.Projects)
            {
                projects.Exists(p =>
                    p.Id == @event.ProjectId &&
                    p.Name == string.Empty).Should().BeTrue();
            }

            projects.Exists(p =>
                p.Id == @event.ProjectId &&
                p.Name == string.Empty).Should().BeTrue();
        }
    }
}
