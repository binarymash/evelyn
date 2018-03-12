namespace Evelyn.Core.Tests.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.AccountProjects;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvents = Core.WriteModel.Account.Events;
    using ProjectEvents = Core.WriteModel.Project.Events;

    public class AccountProjectsHandlerSpecs : HandlerSpecs
    {
        private readonly List<IEvent> _eventsProject1;
        private readonly List<IEvent> _eventsProject2;

        private Guid _accountId;

        private ProjectEvents.ProjectCreated project1CreatedEvent;
        private ProjectEvents.ProjectCreated _project2CreatedEvent;

        private AccountProjectsDto _retrievedAccountProjects;

        public AccountProjectsHandlerSpecs()
        {
            _eventsProject1 = new List<IEvent>();
            _eventsProject2 = new List<IEvent>();
        }

        [Fact]
        public void ProjectCreated()
        {
            this.Given(_ => GivenAnAccountIsRegistered())
                .And(_ => GivenAnProjectIsCreated())
                .When(_ => WhenWeGetTheProjectList())
                .Then(_ => ThenTheProjectIsAddedToTheProjectList())
                .BDDfy();
        }

        [Fact]
        public void MultipleProjectsCreated()
        {
            this.Given(_ => GivenAnAccountIsRegistered())
                .And(_ => GivenAnProjectIsCreated())
                .And(_ => GivenAnotherProjectIsCreated())
                .When(_ => WhenWeGetTheProjectList())
                .Then(_ => ThenBothProjectsAreInTheProjectList())
                .BDDfy();
        }

        protected override void RegisterHandlers(Router router)
        {
            var handler = new AccountProjectsHandler(AccountProjectsStore);
            router.RegisterHandler<AccountEvents.AccountRegistered>(handler.Handle);
            router.RegisterHandler<ProjectEvents.ProjectCreated>(handler.Handle);
        }

        private void GivenAnAccountIsRegistered()
        {
            var accountRegisteredEvent = DataFixture.Create<AccountEvents.AccountRegistered>();
            _accountId = accountRegisteredEvent.Id;
            GivenWePublish(accountRegisteredEvent);
        }

        private void GivenAnProjectIsCreated()
        {
            project1CreatedEvent = DataFixture.Build<ProjectEvents.ProjectCreated>()
                .With(pc => pc.AccountId, _accountId)
                .With(pc => pc.Version, _eventsProject1.Count + 1)
                .Create();

            _eventsProject1.Add(project1CreatedEvent);
            GivenWePublish(project1CreatedEvent);
        }

        private void GivenAnotherProjectIsCreated()
        {
            _project2CreatedEvent = DataFixture.Build<ProjectEvents.ProjectCreated>()
                .With(pc => pc.AccountId, _accountId)
                .With(pc => pc.Version, _eventsProject2.Count + 1)
                .Create();

            _eventsProject2.Add(_project2CreatedEvent);
            GivenWePublish(_project2CreatedEvent);
        }

        private async Task WhenWeGetTheProjectList()
        {
            _retrievedAccountProjects = await ReadModelFacade.GetProjects(_accountId);
        }

        private void ThenTheProjectIsAddedToTheProjectList()
        {
            _retrievedAccountProjects.Projects.Count.Should().Be(1);
            ThenThereIsAnProjectInTheListFor(project1CreatedEvent);
        }

        private void ThenBothProjectsAreInTheProjectList()
        {
            _retrievedAccountProjects.Projects.Count.Should().Be(2);

            ThenThereIsAnProjectInTheListFor(project1CreatedEvent);
            ThenThereIsAnProjectInTheListFor(_project2CreatedEvent);
        }

        private void ThenThereIsAnProjectInTheListFor(ProjectEvents.ProjectCreated ev)
        {
            AccountProjectsStore.Get(_accountId)
                .GetAwaiter().GetResult()
                .Projects[ev.Id].Name.Should().Be(ev.Name);
        }
    }
}
