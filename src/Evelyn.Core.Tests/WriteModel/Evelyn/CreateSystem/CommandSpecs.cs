namespace Evelyn.Core.Tests.WriteModel.Evelyn.CreateSystem
{
    using System;
    using System.Linq;
    using Core.WriteModel.Evelyn.Commands.CreateSystem;
    using Core.WriteModel.Evelyn.Domain;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvent = Core.WriteModel.Account.Events;
    using EvelynEvent = Core.WriteModel.Evelyn.Events;

    public class CommandSpecs : CommandHandlerSpecs<Evelyn, Handler, Command>
    {
        public CommandSpecs()
        {
            UserId = Constants.SystemUser;
        }

        [Fact]
        public void SystemHasNotYetBeenCreated()
        {
            this.When(_ => WhenWeCreateTheSystem())

                .Then(_ => ThenThreeEventsArePublished())

                .And(_ => ThenASystemCreatedEventIsPublishedOnEvelyn())
                .And(_ => ThenAnAccountRegisteredEventIsPublishedOnEvelyn())
                .And(_ => ThenAnAccountRegisteredEventIsPublishedOnTheDefaultAccount())

                .And(_ => ThenTheAggregateRootVersionIsOne())
                .And(_ => ThenTheAggregateRootCreatedTimeHasBeenSet())
                .And(_ => ThenTheAggregateRootCreatedByHasBeenSet())
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())

                .And(_ => ThenTheDefaultAccountHasBeenAddedToTheAggregateRoot())
                .BDDfy();
        }

        [Fact]
        public void SystemHasAlreadyBeenCreated()
        {
            this.Given(_ => GivenWeHaveAlreadyCreatedTheSystem())
                .When(_ => WhenWeCreateTheSystem())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        protected override Handler BuildHandler()
        {
            return new Handler(Session);
        }

        private void GivenWeHaveAlreadyCreatedTheSystem()
        {
            HistoricalEvents.Add(new EvelynEvent.SystemCreated(UserId, Constants.EvelynSystem, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void WhenWeCreateTheSystem()
        {
            var command = new Command(UserId, Constants.EvelynSystem) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void ThenASystemCreatedEventIsPublishedOnEvelyn()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(EvelynEvent.SystemCreated)) as EvelynEvent.SystemCreated;
            @event.UserId.Should().Be(Constants.SystemUser);
            @event.Id.Should().Be(Constants.EvelynSystem);
        }

        private void ThenAnAccountRegisteredEventIsPublishedOnEvelyn()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(EvelynEvent.AccountRegistered)) as EvelynEvent.AccountRegistered;
            @event.UserId.Should().Be(Constants.SystemUser);
            @event.Id.Should().Be(Constants.EvelynSystem);
            @event.AccountId.Should().Be(Constants.DefaultAccount);
        }

        private void ThenAnAccountRegisteredEventIsPublishedOnTheDefaultAccount()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(AccountEvent.AccountRegistered)) as AccountEvent.AccountRegistered;
            @event.UserId.Should().Be(Constants.SystemUser);
            @event.Id.Should().Be(Constants.DefaultAccount);
        }

        private void ThenTheDefaultAccountHasBeenAddedToTheAggregateRoot()
        {
            NewAggregate.Accounts.Count().Should().Be(1);
            NewAggregate.Accounts.First().Should().Be(Constants.DefaultAccount);
        }
    }
}
