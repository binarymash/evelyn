namespace Evelyn.Core.Tests.ReadModel.AccountProjects
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.AccountProjects;
    using Core.ReadModel.Events;
    using Core.ReadModel.ProjectList;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class AccountProjectsHandlerSpecs : HandlerSpecs
    {
        private readonly List<IEvent> _eventsProject1;
        private readonly List<IEvent> _eventsProject2;
        private readonly string _accountId;

        private ProjectCreated _event1;
        private ProjectCreated _event2;

        private AccountProjectsDto _retrievedAccountProjects;

        public AccountProjectsHandlerSpecs()
        {
            _eventsProject1 = new List<IEvent>();
            _eventsProject2 = new List<IEvent>();
            _accountId = DataFixture.Create<string>();
        }

        [Fact]
        public void ProjectCreated()
        {
            this.Given(_ => GivenAnProjectIsCreated())
                .When(_ => WhenWeGetTheProjectList())
                .Then(_ => ThenTheProjectIsAddedToTheProjectList())
                .BDDfy();
        }

        [Fact]
        public void MultipleProjectsCreated()
        {
            this.Given(_ => GivenAnProjectIsCreated())
                .And(_ => GivenAnotherProjectIsCreated())
                .When(_ => WhenWeGetTheProjectList())
                .Then(_ => ThenBothProjectsAreInTheProjectList())
                .BDDfy();
        }

        protected override void RegisterHandlers(Router router)
        {
            var handler = new AccountProjectsHandler(AccountProjectsStore);
            router.RegisterHandler<ProjectCreated>(handler.Handle);
        }

        private void GivenAnProjectIsCreated()
        {
            _event1 = DataFixture.Build<ProjectCreated>()
                .With(pc => pc.AccountId, _accountId)
                .With(pc => pc.Version, _eventsProject1.Count + 1)
                .Create();

            _eventsProject1.Add(_event1);
            GivenWePublish(_event1);
        }

        private void GivenAnotherProjectIsCreated()
        {
            _event2 = DataFixture.Build<ProjectCreated>()
                .With(pc => pc.AccountId, _accountId)
                .With(pc => pc.Version, _eventsProject2.Count + 1)
                .Create();

            _eventsProject2.Add(_event2);
            GivenWePublish(_event2);
        }

        private async Task WhenWeGetTheProjectList()
        {
            _retrievedAccountProjects = await ReadModelFacade.GetProjects(_accountId);
        }

        private void ThenTheProjectIsAddedToTheProjectList()
        {
            _retrievedAccountProjects.Projects.Count.Should().Be(1);
            ThenThereIsAnProjectInTheListFor(_event1);
        }

        private void ThenBothProjectsAreInTheProjectList()
        {
            _retrievedAccountProjects.Projects.Count.Should().Be(2);

            ThenThereIsAnProjectInTheListFor(_event1);
            ThenThereIsAnProjectInTheListFor(_event2);
        }

        private void ThenThereIsAnProjectInTheListFor(ProjectCreated ev)
        {
            AccountProjectsStore.Get(_accountId)
                .GetAwaiter().GetResult()
                .Projects[ev.Id].Name.Should().Be(ev.Name);
        }
    }
}
