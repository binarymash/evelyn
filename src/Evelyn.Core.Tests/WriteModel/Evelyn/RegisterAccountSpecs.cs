namespace Evelyn.Core.Tests.WriteModel.Evelyn
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Evelyn.Commands;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvent = Core.WriteModel.Account.Events;
    using EvelynEvent = Core.WriteModel.Evelyn.Events;

    public class RegisterAccountSpecs : EvelynCommandHandlerSpecs<RegisterAccount>
    {
        private readonly Guid _accountId;

        public RegisterAccountSpecs()
        {
            _accountId = DataFixture.Create<Guid>();
            GivenWeHaveCreatedTheSystem();
        }

        [Fact]
        public void AccountHasNotAlreadyBeenRegistered()
        {
            this.When(_ => WhenWeRegisterAnAccount())

                .Then(_ => ThenTwoEventsArePublished())

                .And(_ => ThenAnAccountRegisteredEventIsPublishedOnEvelyn())
                .And(_ => ThenAnAccountRegisteredEventIsPublishedOnTheAccount())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(4))

                .And(_ => ThenTheAggregateRootHasHadTheAccountAdded())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(1))
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .BDDfy();
        }

        [Fact]
        public void AccountHasAlreadyBeenRegistered()
        {
            this.Given(_ => GivenWeHaveRegisteredAnAccount())
                .When(_ => WhenWeRegisterAnAccount())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenADuplicateAccountExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        private void ThenADuplicateAccountExceptionIsThrown()
        {
            this.ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an account with the ID {_accountId}");
        }

        private void GivenWeHaveCreatedTheSystem()
        {
            HistoricalEvents.Add(new EvelynEvent.SystemCreated(UserId, Constants.EvelynSystem, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void GivenWeHaveRegisteredAnAccount()
        {
            HistoricalEvents.Add(new EvelynEvent.AccountRegistered(UserId, Constants.EvelynSystem, _accountId, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void WhenWeRegisterAnAccount()
        {
            UserId = DataFixture.Create<string>();
            var command = new RegisterAccount(UserId, _accountId);
            WhenWeHandle(command);
        }

        private void ThenAnAccountRegisteredEventIsPublishedOnEvelyn()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(EvelynEvent.AccountRegistered)) as EvelynEvent.AccountRegistered;
            @event.UserId.Should().Be(UserId);
            @event.Id.Should().Be(Constants.EvelynSystem);
            @event.AccountId.Should().Be(_accountId);
        }

        private void ThenAnAccountRegisteredEventIsPublishedOnTheAccount()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(AccountEvent.AccountRegistered)) as AccountEvent.AccountRegistered;
            @event.UserId.Should().Be(UserId);
            @event.Id.Should().Be(_accountId);
        }

        private void ThenTheAggregateRootHasHadTheAccountAdded()
        {
            NewAggregate.Accounts.Should().Contain(_accountId);
        }
    }
}
