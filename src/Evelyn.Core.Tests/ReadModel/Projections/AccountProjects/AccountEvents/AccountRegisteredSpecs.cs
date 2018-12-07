namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.AccountEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel.Account.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class AccountRegisteredSpecs : ProjectionHarness<AccountRegistered>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnAccountRegisteredEvent())
                .Then(_ => ThenTheProjectionIsCreatedWithTheCorrectProperties())
                .And(_ => ThenTheAuditIsCreated())
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

            UpdatedProjection.AccountId.Should().Be(Event.Id);
            UpdatedProjection.Projects.Should().BeEmpty();
        }

        private void ThenTheAccountAuditIsCreated()
        {
            UpdatedProjection.AccountAudit.Created.Should().Be(Event.OccurredAt);
            UpdatedProjection.AccountAudit.CreatedBy.Should().Be(Event.UserId);
            UpdatedProjection.AccountAudit.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.AccountAudit.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.AccountAudit.Version.Should().Be(Event.Version);
        }
    }
}
