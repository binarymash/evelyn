namespace Evelyn.Core.Tests.WriteModel.Evelyn
{
    using System;
    using System.Linq;
    using Core.WriteModel.Evelyn.Commands;
    using CQRSlite.Domain.Exception;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using EvelynEvent = Core.WriteModel.Evelyn.Events;

    public class StartSystemSpecs : EvelynCommandHandlerSpecs<StartSystem>
    {
        [Fact]
        public void SystemHasNotYetBeenCreated()
        {
            this.When(_ => WhenWeStartTheSystem())
                .Then(_ => ThenAnAggregateNotFoundExceptionIsThrown())
                .And(_ => ThenNoEventIsPublished())
                .BDDfy();
        }

        [Fact]
        public void NominalStartUp()
        {
            this.Given(_ => GivenWeHaveCreatedTheSystem())
                .When(_ => WhenWeStartTheSystem())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenASystemStartedEventIsPublished())
                .BDDfy();
        }

        private void GivenWeHaveCreatedTheSystem()
        {
            HistoricalEvents.Add(new EvelynEvent.SystemCreated(UserId, Constants.EvelynSystem, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void WhenWeStartTheSystem()
        {
            var command = new StartSystem(Constants.SystemUser, Constants.EvelynSystem);
            WhenWeHandle(command);
        }

        private void ThenAnAggregateNotFoundExceptionIsThrown()
        {
            ThrownException.Should().BeOfType<AggregateNotFoundException>();
        }

        private void ThenASystemStartedEventIsPublished()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(EvelynEvent.SystemStarted)) as EvelynEvent.SystemStarted;
            @event.UserId.Should().Be(Constants.SystemUser);
            @event.Id.Should().Be(Constants.EvelynSystem);
        }
    }
}