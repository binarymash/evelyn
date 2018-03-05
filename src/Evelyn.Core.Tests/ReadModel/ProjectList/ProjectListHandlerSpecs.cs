namespace Evelyn.Core.Tests.ReadModel.ProjectList
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.ProjectList;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectListHandlerSpecs : HandlerSpecs
    {
        private List<IEvent> _eventsProject1;
        private List<IEvent> _eventsProject2;

        private ProjectCreated _event1;
        private ProjectCreated _event2;

        private List<ProjectListDto> _retrievedProjectList;

        public ProjectListHandlerSpecs()
        {
            _eventsProject1 = new List<IEvent>();
            _eventsProject2 = new List<IEvent>();
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
            var handler = new ProjectListHandler(ProjectsStore);
            router.RegisterHandler<ProjectCreated>(handler.Handle);
        }

        private void GivenAnProjectIsCreated()
        {
            _event1 = DataFixture.Create<ProjectCreated>();
            _event1.Version = _eventsProject1.Count + 1;

            _eventsProject1.Add(_event1);
            GivenWePublish(_event1);
        }

        private void GivenAnotherProjectIsCreated()
        {
            _event2 = DataFixture.Create<ProjectCreated>();
            _event2.Version = _eventsProject2.Count + 1;

            _eventsProject2.Add(_event2);
            GivenWePublish(_event2);
        }

        private async Task WhenWeGetTheProjectList()
        {
            _retrievedProjectList = (await ReadModelFacade.GetProjects()).ToList();
        }

        private void ThenTheProjectIsAddedToTheProjectList()
        {
            _retrievedProjectList.Count.Should().Be(1);
            ThenThereIsAnProjectInTheListFor(_event1);
        }

        private void ThenBothProjectsAreInTheProjectList()
        {
            _retrievedProjectList.Count().Should().Be(2);

            ThenThereIsAnProjectInTheListFor(_event1);
            ThenThereIsAnProjectInTheListFor(_event2);
        }

        private void ThenThereIsAnProjectInTheListFor(ProjectCreated ev)
        {
            ProjectsStore.Get().GetAwaiter().GetResult().Should().Contain(project =>
                project.Id == ev.Id &&
                project.Name == ev.Name);
        }
    }
}
