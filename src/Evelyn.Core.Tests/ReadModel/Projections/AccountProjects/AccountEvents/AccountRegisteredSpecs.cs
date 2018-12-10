namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.AccountEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel.Account.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class AccountRegisteredSpecs : ProjectionBuilderHarness<AccountRegistered>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnAccountRegisteredEvent())
                .Then(_ => ThenTheProjectionIsCreatedWithTheCorrectProperties())
                .And(_ => ThenTheProjectionIsCreated())
                .And(_ => ThenTheAccountAuditIsCreated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private async Task WhenWeHandleAnAccountRegisteredEvent()
        {
            Event = DataFixture.Build<AccountRegistered>()
                .With(ar => ar.Id, AccountId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreatedWithTheCorrectProperties()
        {
            ThenTheProjectionIsCreated();

            UpdatedProjection.Account.AccountId.Should().Be(Event.Id);
            UpdatedProjection.Account.Projects.Should().BeEmpty();
        }

        private void ThenTheAccountAuditIsCreated()
        {
            UpdatedProjection.Account.Audit.Created.Should().Be(Event.OccurredAt);
            UpdatedProjection.Account.Audit.CreatedBy.Should().Be(Event.UserId);
            UpdatedProjection.Account.Audit.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.Account.Audit.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.Account.Audit.Version.Should().Be(Event.Version);
        }
    }
}
