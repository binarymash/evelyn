namespace Evelyn.Core.Tests.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.AccountProjects;
    using Core.WriteModel.Account.Domain;
    using Core.WriteModel.Project.Domain;
    using CQRSlite.Domain.Exception;
    using CQRSlite.Events;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;

    using AccountEvents = Core.WriteModel.Account.Events;
    using ProjectEvents = Core.WriteModel.Project.Events;

    public class ProjectionBuilderSpecs : ReadModel.ProjectionBuilderSpecs<ProjectionBuilder, AccountProjectsDto>
    {
        private readonly List<IEvent> _accountEvents;
        private readonly List<IEvent> _project1Events;
        private readonly List<IEvent> _project2Events;

        private Guid _accountId;
        private Account _account;

        private Guid _project1Id;
        private Project _project1;

        private Guid _project2Id;
        private Project _project2;

        public ProjectionBuilderSpecs()
        {
            Builder = new ProjectionBuilder(SubstituteRepository);
            _accountEvents = new List<IEvent>();
            _project1Events = new List<IEvent>();
            _project2Events = new List<IEvent>();
        }

        [Fact]
        public void AccountDoesNotExist()
        {
            this.Given(_ => GivenTheAccountIsNotInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ProjectDoesNotExist()
        {
            this.Given(_ => GivenWeHaveAnAccountWithProjects())
                .And(_ => GivenTheAccountIsInTheRepository())
                .And(_ => GivenTheProjectsAreNotInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void AccountAndProjectsExist()
        {
            this.Given(_ => GivenWeHaveAnAccountWithProjects())
                .And(_ => GivenTheAccountIsInTheRepository())
                .And(_ => GivenTheProjectsAreInTheRepository())
                .When(_ => WhenWeInvokeTheProjectionBuilder())
                .Then(_ => ThenTheAccountIdIsSet())
                .And(_ => ThenTheVersionIsSet())
                .And(_ => ThenTheCreatedDateIsSet())
                .And(_ => ThenTheCreatedByIsSet())
                .And(_ => ThenTheLastModifiedDateIsSet())
                .And(_ => ThenTheLastModifiedByIsSet())
                .And(_ => ThenAllTheProjectsAreSet())
                .BDDfy();
        }

        private void GivenTheAccountIsNotInTheRepository()
        {
            _accountId = DataFixture.Create<Guid>();

            SubstituteRepository
                .Get<Account>(_accountId, Arg.Any<CancellationToken>())
                .Throws(new AggregateNotFoundException(typeof(Account), _accountId));
        }

        private void GivenTheAccountIsInTheRepository()
        {
            SubstituteRepository
                .Get<Account>(_accountId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(_account));
        }

        private void GivenWeHaveAnAccountWithProjects()
        {
            _accountId = DataFixture.Create<Guid>();

            _project1Id = DataFixture.Create<Guid>();
            _project2Id = DataFixture.Create<Guid>();

            _accountEvents.Add(DataFixture.Build<AccountEvents.AccountRegistered>()
                .With(ev => ev.Version, 0)
                .With(ev => ev.Id, _accountId)
                .Create());

            _accountEvents.Add(DataFixture.Build<AccountEvents.ProjectCreated>()
                .With(ev => ev.Version, 1)
                .With(ev => ev.Id, _accountId)
                .With(ev => ev.ProjectId, _project1Id)
                .Create());

            _accountEvents.Add(DataFixture.Build<AccountEvents.ProjectCreated>()
                .With(ev => ev.Version, 2)
                .With(ev => ev.Id, _accountId)
                .With(ev => ev.ProjectId, _project2Id)
                .Create());

            _account = new Account();
            _account.LoadFromHistory(_accountEvents);

            _project1Events.Add(DataFixture.Build<ProjectEvents.ProjectCreated>()
                .With(ev => ev.Version, 0)
                .With(ev => ev.Id, _project1Id)
                .Create());

            _project1 = new Project();
            _project1.LoadFromHistory(_project1Events);

            _project2Events.Add(DataFixture.Build<ProjectEvents.ProjectCreated>()
                .With(ev => ev.Version, 0)
                .With(ev => ev.Id, _project2Id)
                .Create());

            _project2 = new Project();
            _project2.LoadFromHistory(_project2Events);
        }

        private void GivenTheProjectsAreNotInTheRepository()
        {
            SubstituteRepository
                .Get<Project>(_project1Id, Arg.Any<CancellationToken>())
                .Throws(new AggregateNotFoundException(typeof(Account), _project1Id));

            SubstituteRepository
                .Get<Project>(_project2Id, Arg.Any<CancellationToken>())
                .Throws(new AggregateNotFoundException(typeof(Account), _project2Id));
        }

        private void GivenTheProjectsAreInTheRepository()
        {
            SubstituteRepository
                .Get<Project>(_project1Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(_project1));

            SubstituteRepository
                .Get<Project>(_project2Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(_project2));
        }

        private async Task WhenWeInvokeTheProjectionBuilder()
        {
            try
            {
                var request = new ProjectionBuilderRequest(_accountId);
                Dto = await Builder.Invoke(request);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenTheAccountIdIsSet()
        {
            Dto.AccountId.Should().Be(_accountId);
        }

        private void ThenTheVersionIsSet()
        {
            Dto.Version.Should().Be(_account.Version);
        }

        private void ThenTheCreatedDateIsSet()
        {
            Dto.Created.Should().Be(_account.Created);
        }

        private void ThenTheCreatedByIsSet()
        {
            Dto.CreatedBy.Should().Be(_account.CreatedBy);
        }

        private void ThenTheLastModifiedDateIsSet()
        {
            Dto.LastModified.Should().Be(_account.LastModified);
        }

        private void ThenTheLastModifiedByIsSet()
        {
            Dto.LastModifiedBy.Should().Be(_account.LastModifiedBy);
        }

        private void ThenAllTheProjectsAreSet()
        {
            var projects = Dto.Projects.ToList();

            projects.Count.Should().Be(_account.Projects.Count());

            projects.Exists(p => p.Id == _project1.Id && p.Name == _project1.Name).Should().BeTrue();
            projects.Exists(p => p.Id == _project2.Id && p.Name == _project2.Name).Should().BeTrue();
        }
    }
}
