namespace Evelyn.Management.Api.Rest.Tests.Read
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Management.Api.Rest.Read;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;

    public class ApplicationsControllerSpecs
    {
        private readonly Fixture _fixture;
        private readonly ApplicationsController _controller;
        private readonly IReadModelFacade _readModelFacade;
        private Guid _idOfApplicationToGet;
        private IEnumerable<ApplicationListDto> _applicationsReturnedByFacade;
        private ApplicationDetailsDto _applicationReturnedByFacade;
        private ObjectResult _result;

        public ApplicationsControllerSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new ApplicationsController(_readModelFacade);
        }

        [Fact]
        public void GetsAllApplications()
        {
            this.Given(_ => GivenThatThereAreApplications())
                .When(_ => WhenWeGetAllTheApplications())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenAllApplicationsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingApplications()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownByHandlerWhenGettingApplications())
                .When(_ => WhenWeGetAllTheApplications())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        [Fact]
        public void GetsApplication()
        {
            this.Given(_ => GivenTheApplicationWeWantDoesExist())
                .When(_ => WhenWeGetTheApplication())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenTheExpectedApplicationIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ApplicationNotFound()
        {
            this.Given(_ => GivenTheApplicationWeWantDoesntExist())
                .When(_ => WhenWeGetTheApplication())
                .Then(_ => ThenStatusCode404IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingApplication()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownWhenGettingApplication())
                .When(_ => WhenWeGetTheApplication())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        private void GivenThatThereAreApplications()
        {
            _applicationsReturnedByFacade = _fixture.CreateMany<ApplicationListDto>();

            _readModelFacade.GetApplications().Returns(_applicationsReturnedByFacade);
        }

        private void GivenThatAnExceptionIsThrownByHandlerWhenGettingApplications()
        {
            _readModelFacade
                .GetApplications()
                .Throws(_fixture.Create<Exception>());
        }

        private void GivenTheApplicationWeWantDoesExist()
        {
            _applicationReturnedByFacade = _fixture.Create<ApplicationDetailsDto>();
            _idOfApplicationToGet = _applicationReturnedByFacade.Id;
            _readModelFacade
                .GetApplicationDetails(_idOfApplicationToGet)
                .Returns(_applicationReturnedByFacade);
        }

        private void GivenTheApplicationWeWantDoesntExist()
        {
            _idOfApplicationToGet = _fixture.Create<Guid>();
            _readModelFacade
                .GetApplicationDetails(_idOfApplicationToGet)
                .Throws(_fixture.Create<NotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingApplication()
        {
            _idOfApplicationToGet = _fixture.Create<Guid>();
            _readModelFacade
                .GetApplicationDetails(_idOfApplicationToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetAllTheApplications()
        {
            _result = await _controller.Get() as ObjectResult;
        }

        private async Task WhenWeGetTheApplication()
        {
            _result = await _controller.Get(_idOfApplicationToGet) as ObjectResult;
        }

        private void ThenStatusCode200IsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        private void ThenStatusCode404IsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        private void ThenStatusCode500IsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        private void ThenAllApplicationsAreReturned()
        {
            var returnedApplications = (_result.Value as IEnumerable<ApplicationListDto>).ToList();

            returnedApplications.Should().Equal(_applicationsReturnedByFacade);
        }

        private void ThenTheExpectedApplicationIsReturned()
        {
            var returnedApplication = _result.Value as ApplicationDetailsDto;

            returnedApplication.Should().Be(_applicationReturnedByFacade);
        }
    }
}
