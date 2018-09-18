////namespace Evelyn.Core.Tests.ReadModel.ProjectDetails
////{
////    using System;
////    using System.Collections.Generic;
////    using System.Linq;
////    using System.Threading;
////    using System.Threading.Tasks;
////    using AutoFixture;
////    using Core.ReadModel.ProjectDetails;
////    using Core.WriteModel.Project.Domain;
////    using Core.WriteModel.Project.Events;
////    using CQRSlite.Domain.Exception;
////    using CQRSlite.Events;
////    using FluentAssertions;
////    using NSubstitute;
////    using NSubstitute.ExceptionExtensions;
////    using TestStack.BDDfy;
////    using Xunit;

////    public class ProjectionBuilderSpecs : ProjectionBuilderSpecs<ProjectionBuilder, ProjectDetailsDto>
////    {
////        private readonly List<IEvent> _projectEvents;

////        private Guid _projectId;
////        private Project _project;

////        public ProjectionBuilderSpecs()
////        {
////            _projectEvents = new List<IEvent>();
////            Builder = new ProjectionBuilder(SubstituteRepository);
////        }

////        [Fact]
////        public void ProjectDoesntExist()
////        {
////            this.Given(_ => GivenTheProjectDoesNotExistInTheRepository())
////                .When(_ => WhenWeInvokeTheProjectionBuilder())
////                .Then(_ => ThenANullProjectionIsReturned())
////                .BDDfy();
////        }

////        [Fact]
////        public void EnvironmentExists()
////        {
////            this.Given(_ => GivenWeHaveAProjectWithEnvironmentsAndToggles())
////                .And(_ => GivenTheProjectIsInTheRepository())
////                .When(_ => WhenWeInvokeTheProjectionBuilder())
////                .Then(_ => ThenTheCreatedDateIsSet())
////                .And(_ => ThenTheCreatedByIsSet())
////                .And(_ => ThenTheLastModifiedDateIsSet())
////                .And(_ => ThenTheLastModifiedByIsSet())
////                .And(_ => ThenTheVersionIsSet())
////                .And(_ => ThenTheIdIsSet())
////                .And(_ => ThenTheNameIsSet())
////                .And(_ => ThenAllTheEnvironmentsAreSet())
////                .And(_ => ThenAllTheTogglesAreSet())
////                .BDDfy();
////        }

////        [Fact]
////        public void ProjectHasBeenDeleted()
////        {
////            this.Given(_ => GivenWeHaveAProjectThatHasBeenDeleted())
////                .And(_ => GivenTheProjectIsInTheRepository())
////                .When(_ => WhenWeInvokeTheProjectionBuilder())
////                .Then(_ => ThenANullProjectionIsReturned())
////                .BDDfy();
////        }

////        [Fact]
////        public void SomeOtherExceptionFromEventStore()
////        {
////            this.Given(_ => GivenTheEventStoreThrowsSomeOtherException())
////                .When(_ => WhenWeInvokeTheProjectionBuilder())
////                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
////                .BDDfy();
////        }

////        private void GivenTheProjectDoesNotExistInTheRepository()
////        {
////            _projectId = DataFixture.Create<Guid>();

////            SubstituteRepository
////                .Get<Project>(_projectId, Arg.Any<CancellationToken>())
////                .Throws(new AggregateNotFoundException(typeof(Project), _projectId));
////        }

////        private void GivenTheProjectIsInTheRepository()
////        {
////            SubstituteRepository
////                .Get<Project>(_projectId, Arg.Any<CancellationToken>())
////                .Returns(Task.FromResult(_project));
////        }

////        private void GivenWeHaveAProjectThatHasBeenDeleted()
////        {
////            _projectId = DataFixture.Create<Guid>();

////            _projectEvents.Add(DataFixture.Build<ProjectCreated>()
////                .With(ev => ev.Version, 0)
////                .With(ev => ev.Id, _projectId)
////                .Create());

////            _projectEvents.Add(DataFixture.Build<ProjectDeleted>()
////                .With(ev => ev.Version, 1)
////                .With(ev => ev.Id, _projectId)
////                .Create());

////            _project = new Project();
////            _project.LoadFromHistory(_projectEvents);
////        }

////        private void GivenWeHaveAProjectWithEnvironmentsAndToggles()
////        {
////            _projectId = DataFixture.Create<Guid>();

////            _projectEvents.Add(DataFixture.Build<ProjectCreated>()
////                .With(ev => ev.Version, 0)
////                .With(ev => ev.Id, _projectId)
////                .Create());

////            _projectEvents.Add(DataFixture.Build<EnvironmentAdded>()
////                .With(ev => ev.Version, 1)
////                .With(ev => ev.Id, _projectId)
////                .Create());

////            _projectEvents.Add(DataFixture.Build<ToggleAdded>()
////                .With(ev => ev.Version, 2)
////                .With(ev => ev.Id, _projectId)
////                .Create());

////            _projectEvents.Add(DataFixture.Build<ToggleAdded>()
////                .With(ev => ev.Version, 3)
////                .With(ev => ev.Id, _projectId)
////                .Create());

////            _projectEvents.Add(DataFixture.Build<ToggleAdded>()
////                .With(ev => ev.Version, 4)
////                .With(ev => ev.Id, _projectId)
////                .Create());

////            _projectEvents.Add(DataFixture.Build<EnvironmentAdded>()
////                .With(ev => ev.Version, 5)
////                .With(ev => ev.Id, _projectId)
////                .Create());

////            _projectEvents.Add(DataFixture.Build<ToggleAdded>()
////                .With(ev => ev.Version, 6)
////                .With(ev => ev.Id, _projectId)
////                .Create());

////            _project = new Project();
////            _project.LoadFromHistory(_projectEvents);
////        }

////        private void GivenTheEventStoreThrowsSomeOtherException()
////        {
////            SubstituteRepository
////                .Get<Project>(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
////                .Throws(DataFixture.Create<Exception>());
////        }

////        private async Task WhenWeInvokeTheProjectionBuilder()
////        {
////            try
////            {
////                Dto = await Builder.Invoke(new ProjectionBuilderRequest(_projectId));
////            }
////            catch (Exception ex)
////            {
////                ThrownException = ex;
////            }
////        }

////        private void ThenTheCreatedDateIsSet()
////        {
////            Dto.Created.Should().Be(_project.Created);
////        }

////        private void ThenTheCreatedByIsSet()
////        {
////            Dto.CreatedBy.Should().Be(_project.CreatedBy);
////        }

////        private void ThenTheLastModifiedDateIsSet()
////        {
////            Dto.LastModified.Should().Be(_project.LastModified);
////        }

////        private void ThenTheLastModifiedByIsSet()
////        {
////            Dto.LastModifiedBy.Should().Be(_project.LastModifiedBy);
////        }

////        private void ThenTheVersionIsSet()
////        {
////            Dto.Version.Should().Be(_project.LastModifiedVersion);
////        }

////        private void ThenTheIdIsSet()
////        {
////            Dto.Id.Should().Be(_project.Id);
////        }

////        private void ThenTheNameIsSet()
////        {
////            Dto.Name.Should().Be(_project.Name);
////        }

////        private void ThenAllTheEnvironmentsAreSet()
////        {
////            var environments = Dto.Environments.ToList();

////            environments.Count.Should().Be(_project.Environments.Count());

////            foreach (var environment in _project.Environments)
////            {
////                environments.Exists(e => e.Key == environment.Key).Should().BeTrue();
////            }
////        }

////        private void ThenAllTheTogglesAreSet()
////        {
////            var toggles = Dto.Toggles.ToList();

////            toggles.Count.Should().Be(_project.Toggles.Count());

////            foreach (var toggle in _project.Toggles)
////            {
////                toggles.Exists(e => e.Key == toggle.Key && e.Name == toggle.Name).Should().BeTrue();
////            }
////        }
////    }
////}
