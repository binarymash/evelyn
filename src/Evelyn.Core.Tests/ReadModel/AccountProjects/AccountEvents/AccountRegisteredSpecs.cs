namespace Evelyn.Core.Tests.ReadModel.AccountProjects.AccountEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel.Account.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class AccountRegisteredSpecs : EventSpecs
    {
        private AccountRegistered _event;

        [Fact]
        public void NoProjection()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnAccountRegisteredEvent())
                .Then(_ => ThenAProjectionHasBeenAddedToTheStore())
                .BDDfy();
        }

        [Fact]
        public void ProjectionAlreadyExists()
        {
            this.Given(_ => GivenTheProjectionExists())
                .When(_ => WhenWeHandleAnAccountRegisteredEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByProjectionStoreWhenSaving()
        {
            this.Given(_ => GivenTheProjectionStoreWillThrowWhenCreating())
                .When(_ => WhenWeHandleAnAccountRegisteredEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAnAccountRegisteredEvent()
        {
            _event = DataFixture.Build<AccountRegistered>()
                .With(ar => ar.Id, AccountId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private async Task ThenAProjectionHasBeenAddedToTheStore()
        {
            var projection = await ProjectionStore.Get(AccountProjectsDto.StoreKey(_event.Id));
            projection.AccountId.Should().Be(_event.Id);
            projection.Created.Should().Be(_event.OccurredAt);
            projection.CreatedBy.Should().Be(_event.UserId);
            projection.LastModified.Should().Be(_event.OccurredAt);
            projection.LastModifiedBy.Should().Be(_event.UserId);
            projection.Projects.Should().BeEmpty();
            projection.Version.Should().Be(0);
        }
    }
}
