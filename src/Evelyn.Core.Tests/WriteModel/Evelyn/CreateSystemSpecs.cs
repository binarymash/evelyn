namespace Evelyn.Core.Tests.WriteModel.Evelyn
{
    using System.Linq;
    using Core.WriteModel.Evelyn.Commands;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvent = Core.WriteModel.Account.Events;
    using EvelynEvent = Core.WriteModel.Evelyn.Events;

    public class CreateSystemSpecs : EvelynCommandHandlerSpecs<CreateSystem>
    {
        [Fact]
        public void SystemHasNotYetBeenCreated()
        {
            this.When(_ => WhenWeCreateTheSystem())
                .Then(_ => ThenThreeEventsArePublished())
                .And(_ => ThenASystemCreatedEventIsPublishedOnEvelyn())
                .And(_ => ThenAnAccountRegisteredEventIsPublishedOnEvelyn())
                .And(_ => ThenAnAccountRegisteredEventIsPublishedOnTheDefaultAccount())
                .BDDfy();
        }

        [Fact]
        public void SystemHasAlreadyBeenCreated()
        {
            this.Given(_ => GivenWeHaveAlreadyCreatedTheSystem())
                .When(_ => WhenWeCreateTheSystem())
                .Then(_ => ThenNoEventIsPublished())
                .BDDfy();
        }

        private void GivenWeHaveAlreadyCreatedTheSystem()
        {
            HistoricalEvents.Add(new EvelynEvent.SystemCreated(UserId, Constants.EvelynSystem) { Version = HistoricalEvents.Count });
        }

        private void WhenWeCreateTheSystem()
        {
            var command = new CreateSystem(UserId, Constants.EvelynSystem) { ExpectedVersion = HistoricalEvents.Count - 1 };
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
    }
}