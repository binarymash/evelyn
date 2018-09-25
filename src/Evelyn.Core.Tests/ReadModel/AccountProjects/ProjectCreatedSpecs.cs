namespace Evelyn.Core.Tests.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel.Account.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectCreatedSpecs
    {
        private Fixture _fixture;
        private IProjectionStore<AccountProjectsDto> _projectionStore;
        private ProjectCreated @event;
        private ProjectionBuilder _projectionBuilder;
        private CancellationToken _stoppingToken;
        private Guid _accountId;
        private AccountProjectsDto _originalProjection;
        private Exception _thrownException;
        private AccountProjectsDto _updatedProjection;

        public ProjectCreatedSpecs()
        {
            _fixture = new Fixture();
            _projectionStore = new InMemoryProjectionStore<AccountProjectsDto>();
            _stoppingToken = default;

            _accountId = Guid.NewGuid();
            @event = _fixture.Build<ProjectCreated>()
                .With(pc => pc.Id, _accountId)
                .Create();
        }

        [Fact]
        public void AccountDoesntExist()
        {
            this.Given(_ => GivenNoAccountProjectsProjectionExists())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void AccountExistsWithNoProjects()
        {
            this.Given(_ => GivenAnAccountExistsWithNoProjects())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenTheProjectionHasBeenUpdated())
                .BDDfy();
        }

        [Fact]
        public void AccountExistsWithAProject()
        {
            this.Given(_ => GivenAnAccountExistsWithAProject())
                .When(_ => WhenTheEventIsHandled())
                .Then(_ => ThenTheProjectionHasBeenUpdated2())
                .BDDfy();
        }

        private void GivenNoAccountProjectsProjectionExists()
        {
            _originalProjection = null;
        }

        private async Task GivenAnAccountExistsWithNoProjects()
        {
            _originalProjection = AccountProjectsDto.Create(
                _accountId,
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());

            await _projectionStore.AddOrUpdate(AccountProjectsDto.StoreKey(_originalProjection.AccountId), _originalProjection);
        }

        private async Task GivenAnAccountExistsWithAProject()
        {
            _originalProjection = AccountProjectsDto.Create(
                _accountId,
                _fixture.Create<DateTimeOffset>(),
                _fixture.Create<string>());

            _originalProjection.AddProject(
                _fixture.Create<Guid>(),
                _fixture.Create<string>(),
                _fixture.Create<int>(),
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

        private void ThenTheProjectionHasBeenUpdated()
        {
            _updatedProjection.LastModified.Should().Be(@event.OccurredAt);
            _updatedProjection.LastModifiedBy.Should().Be(@event.UserId);
            _updatedProjection.Version.Should().Be(@event.Version);

            _updatedProjection.Projects.Count().Should().Be(1);
            var project = _updatedProjection.Projects.First();
            project.Id.Should().Be(@event.ProjectId);
            project.Name.Should().Be(string.Empty);

            _updatedProjection.AccountId.Should().Be(_originalProjection.AccountId);
            _updatedProjection.Created.Should().Be(_originalProjection.Created);
            _updatedProjection.CreatedBy.Should().Be(_originalProjection.CreatedBy);
        }

        private void ThenTheProjectionHasBeenUpdated2()
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
