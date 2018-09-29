namespace Evelyn.Core.Tests.ReadModel.AccountProjects.AccountEvents
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvents = Evelyn.Core.WriteModel.Account.Events;

    public class AccountRegisteredSpecs
    {
        private readonly Fixture _fixture;
        private ProjectionBuilder _projectionBuilder;
        private CancellationToken _stoppingToken;
        private IProjectionStore<AccountProjectsDto> _projectionStore;
        private Guid _accountId;
        private AccountProjectsDto _originalProjection;
        private AccountEvents.AccountRegistered @event;
        private Exception _exceptionFromStore;
        private Exception _thrownException;

        public AccountRegisteredSpecs()
        {
            _fixture = new Fixture();
            _projectionStore = new InMemoryProjectionStore<AccountProjectsDto>();
            _projectionBuilder = new ProjectionBuilder(_projectionStore);
            _stoppingToken = default;
        }

        [Fact]
        public void NoProjection()
        {
            this.Given(_ => GivenTheProjectionDoesNotAlreadyExist())
                .When(_ => WhenWeHandleAnAccountRegisteredEvent())
                .Then(_ => ThenAProjectionHasBeenAddedToTheStore())
                .BDDfy();
        }

        [Fact]
        public void ProjectionAlreadyExists()
        {
            this.Given(_ => GivenTheProjectionAlreadyExists())
                .When(_ => WhenWeHandleAnAccountRegisteredEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByProjectionStoreWhenSaving()
        {
            this.Given(_ => GivenTheProjectionStoreWillThrowWhenSaving())
                .When(_ => WhenWeHandleAnAccountRegisteredEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        private void GivenTheProjectionDoesNotAlreadyExist()
        {
            _accountId = _fixture.Create<Guid>();
        }

        private void GivenTheProjectionAlreadyExists()
        {
            _originalProjection = _fixture.Create<AccountProjectsDto>();
            _accountId = _originalProjection.AccountId;
        }

        private void GivenTheProjectionStoreWillThrowWhenSaving()
        {
            _projectionStore = Substitute.For<IProjectionStore<AccountProjectsDto>>();

            _exceptionFromStore = new Exception();
            _projectionStore.Create(Arg.Any<string>(), Arg.Any<AccountProjectsDto>())
                .Returns(ps => throw _exceptionFromStore);
        }

        private async Task WhenWeHandleAnAccountRegisteredEvent()
        {
            @event = _fixture.Build<AccountEvents.AccountRegistered>()
                .With(ar => ar.Id, _accountId)
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
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        private async Task ThenAProjectionHasBeenAddedToTheStore()
        {
            var projection = await _projectionStore.Get(AccountProjectsDto.StoreKey(@event.Id));
            projection.AccountId.Should().Be(@event.Id);
            projection.Created.Should().Be(@event.OccurredAt);
            projection.CreatedBy.Should().Be(@event.UserId);
            projection.LastModified.Should().Be(@event.OccurredAt);
            projection.LastModifiedBy.Should().Be(@event.UserId);
            projection.Projects.Should().BeEmpty();
            projection.Version.Should().Be(0);
        }

        private void ThenAnExceptionIsThrown()
        {
            _thrownException.Should().NotBeNull();
        }
    }
}
