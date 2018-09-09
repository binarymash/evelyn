namespace Evelyn.Core.Tests.WriteModel.Evelyn.RegisterAccount
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Evelyn.Commands.RegisterAccount;
    using Core.WriteModel.Evelyn.Domain;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvent = Core.WriteModel.Account.Events;
    using EvelynEvent = Core.WriteModel.Evelyn.Events;

    public class CommandSpecs : CommandHandlerSpecs<Evelyn, Handler, Command>
    {
        private readonly Guid _accountId;

        public CommandSpecs()
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

        protected override Handler BuildHandler()
        {
            return new Handler(Logger, Session);
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
            var command = new Command(UserId, _accountId);
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
