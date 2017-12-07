namespace Evelyn.Api.Rest.Tests.Read
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Api.Rest.Read.Areas;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class ApplicationsControllerSpecs
    {
        private readonly Fixture _fixture;
        private readonly ApplicationsController _controller;
        private readonly IReadModelFacade _readModelFacade;
        private IEnumerable<ApplicationListDto> _applicationsReturnedByFacade;
        private IActionResult _result;

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

        private void GivenThatThereAreApplications()
        {
            _applicationsReturnedByFacade = _fixture.CreateMany<ApplicationListDto>();

            _readModelFacade.GetApplications().Returns(_applicationsReturnedByFacade);
        }

        private void GivenThatAnExceptionIsThrownByHandlerWhenGettingApplications()
        {
            _readModelFacade.GetApplications().Throws(new Exception("boom"));
        }

        private async Task WhenWeGetAllTheApplications()
        {
            _result = await _controller.Get();
        }

        private void ThenStatusCode200IsReturned()
        {
            ((ObjectResult)_result).StatusCode.ShouldBe(StatusCodes.Status200OK);
        }

        private void ThenStatusCode500IsReturned()
        {
            ((StatusCodeResult)_result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }

        private void ThenAllApplicationsAreReturned()
        {
            var returnedApplications = (((ObjectResult)_result).Value as IEnumerable<ApplicationListDto>).ToList();

            returnedApplications.ShouldBe(_applicationsReturnedByFacade);
        }
    }
}
