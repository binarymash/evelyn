namespace Evelyn.Api.Rest.Tests.Read
{
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
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class ApplicationsControllerSpecs
    {
        private Fixture _fixture;
        private ApplicationsController _controller;
        private IReadModelFacade _readModelFacade;
        private IEnumerable<ApplicationListDto> _applicationsReturnedByFacade;
        private ObjectResult _result;

        public ApplicationsControllerSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new ApplicationsController(_readModelFacade);
        }

        [Fact]
        public void DoesSomething()
        {
            this.Given(_ => GivenThatThereAreApplications())
                .When(_ => WhenWeGetAllTheApplications())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenAllApplicationsAreReturned())
                .BDDfy();
        }

        private void GivenThatThereAreApplications()
        {
            _applicationsReturnedByFacade = _fixture.CreateMany<ApplicationListDto>();

            _readModelFacade.GetApplications().Returns(_applicationsReturnedByFacade);
        }

        private async Task WhenWeGetAllTheApplications()
        {
            _result = (await _controller.Get()) as ObjectResult;
        }

        private void ThenStatusCode200IsReturned()
        {
            _result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        }

        private void ThenAllApplicationsAreReturned()
        {
            var returnedApplications = (_result.Value as IEnumerable<ApplicationListDto>).ToList();

            returnedApplications.ShouldBe(_applicationsReturnedByFacade);
        }
    }
}
