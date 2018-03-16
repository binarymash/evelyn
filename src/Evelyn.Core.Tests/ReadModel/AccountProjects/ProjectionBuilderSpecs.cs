namespace Evelyn.Core.Tests.ReadModel.AccountProjects
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.AccountProjects;
    using Core.WriteModel.Account.Domain;
    using Core.WriteModel.Project.Domain;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    using AccountEvents = Core.WriteModel.Account.Events;
    using ProjectEvents = Core.WriteModel.Project.Events;

    public class ProjectionBuilderSpecs : ReadModel.ProjectionBuilderSpecs
    {
        private readonly ProjectionBuilder _builder;

        private Guid _accountId;

        private ProjectEvents.ProjectCreated _project1CreatedEvent;
        private ProjectEvents.ProjectCreated _project2CreatedEvent;

        private AccountProjectsDto _dto;

        public ProjectionBuilderSpecs()
        {
            _builder = new ProjectionBuilder(StubbedRepository);
        }

        [Fact]
        public void ProjectCreated()
        {
            this.Given(_ => GivenAnAccountIsRegistered())
                .And(_ => GivenAnProjectIsCreated())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenTheProjectIsAddedToTheProjectList())
                .BDDfy();
        }

        [Fact]
        public void MultipleProjectsCreated()
        {
            this.Given(_ => GivenAnAccountIsRegistered())
                .And(_ => GivenAnProjectIsCreated())
                .And(_ => GivenAnotherProjectIsCreated())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenBothProjectsAreInTheProjectList())
                .BDDfy();
        }

        private void GivenAnAccountIsRegistered()
        {
            var accountRegisteredEvent = DataFixture.Create<AccountEvents.AccountRegistered>();
            accountRegisteredEvent.Version = StubbedRepository.NextVersionFor(accountRegisteredEvent.Id);

            _accountId = accountRegisteredEvent.Id;

            StubbedRepository.AddEvent(accountRegisteredEvent, () => new Account());
        }

        private void GivenAnProjectIsCreated()
        {
            var projectCreatedOnAccount = DataFixture.Build<AccountEvents.ProjectCreated>()
                .With(pc => pc.Id, _accountId)
                .With(pc => pc.Version, StubbedRepository.NextVersionFor(_accountId))
                .Create();

            StubbedRepository.AddEvent(projectCreatedOnAccount);

            _project1CreatedEvent = new ProjectEvents.ProjectCreated(
                projectCreatedOnAccount.UserId,
                projectCreatedOnAccount.Id,
                projectCreatedOnAccount.ProjectId,
                DataFixture.Create<string>());

            _project1CreatedEvent.Version = StubbedRepository.NextVersionFor(_project1CreatedEvent.Id);

            StubbedRepository.AddEvent(_project1CreatedEvent, () => new Project());
        }

        private void GivenAnotherProjectIsCreated()
        {
            var projectCreatedOnAccount = DataFixture.Build<AccountEvents.ProjectCreated>()
                .With(pc => pc.Id, _accountId)
                .With(pc => pc.Version, StubbedRepository.NextVersionFor(_accountId))
                .Create();

            StubbedRepository.AddEvent(projectCreatedOnAccount);

            _project2CreatedEvent = new ProjectEvents.ProjectCreated(
                projectCreatedOnAccount.UserId,
                projectCreatedOnAccount.Id,
                projectCreatedOnAccount.ProjectId,
                DataFixture.Create<string>());

            _project2CreatedEvent.Version = StubbedRepository.NextVersionFor(_project2CreatedEvent.Id);

            StubbedRepository.AddEvent(_project2CreatedEvent, () => new Project());
        }

        private async Task WhenWeInvokeTheProjectionBuilder()
        {
            try
            {
                var request = new ProjectionBuilderRequest(_accountId);
                _dto = await _builder.Invoke(request);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenTheProjectIsAddedToTheProjectList()
        {
            _dto.Projects.Count().Should().Be(1);
            ThenThereIsAnProjectInTheListFor(_project1CreatedEvent);
        }

        private void ThenBothProjectsAreInTheProjectList()
        {
            _dto.Projects.Count().Should().Be(2);

            ThenThereIsAnProjectInTheListFor(_project1CreatedEvent);
            ThenThereIsAnProjectInTheListFor(_project2CreatedEvent);
        }

        private void ThenThereIsAnProjectInTheListFor(ProjectEvents.ProjectCreated ev)
        {
            var project = _dto.Projects.First(p => p.Id == ev.Id);
            project.Name.Should().Be(ev.Name);
        }
    }
}
