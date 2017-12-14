namespace Evelyn.Management.Api.Rest.Tests.Read
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Management.Api.Rest.Read;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentsControllerSpecs
    {
        private readonly Fixture _fixture;
        private readonly IReadModelFacade _readModelFacade;
        private readonly EnvironmentsController _controller;
        private EnvironmentDetailsDto _environmentReturnedByFacade;
        private Guid _idOfEnvironmentToGet;
        private IActionResult _result;

        public EnvironmentsControllerSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new EnvironmentsController(_readModelFacade);
        }

        [Fact]
        public void GetsEnvironment()
        {
            this.Given(_ => GivenTheEnvironmentWeWantDoesExist())
                .When(_ => WhenWeGetTheEnvironment())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenTheExpectedEnvironmentIsReturned())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentNotFound()
        {
            this.Given(_ => GivenTheEnvironmentWeWantDoesntExist())
                .When(_ => WhenWeGetTheEnvironment())
                .Then(_ => ThenStatusCode404IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingEnvironment()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownWhenGettingEnvironment())
                .When(_ => WhenWeGetTheEnvironment())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        private void GivenTheEnvironmentWeWantDoesExist()
        {
            _environmentReturnedByFacade = _fixture.Create<EnvironmentDetailsDto>();
            _idOfEnvironmentToGet = _environmentReturnedByFacade.Id;
            _readModelFacade
                .GetEnvironmentDetails(_idOfEnvironmentToGet)
                .Returns(_environmentReturnedByFacade);
        }

        private void GivenTheEnvironmentWeWantDoesntExist()
        {
            _idOfEnvironmentToGet = _fixture.Create<Guid>();
            _readModelFacade
                .GetEnvironmentDetails(_idOfEnvironmentToGet)
                .Throws(_fixture.Create<NotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingEnvironment()
        {
            _idOfEnvironmentToGet = _fixture.Create<Guid>();
            _readModelFacade
                .GetEnvironmentDetails(_idOfEnvironmentToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetTheEnvironment()
        {
            _result = await _controller.Get(_idOfEnvironmentToGet);
        }

        private void ThenStatusCode200IsReturned()
        {
            ((ObjectResult)_result).StatusCode.ShouldBe(StatusCodes.Status200OK);
        }

        private void ThenStatusCode404IsReturned()
        {
            ((StatusCodeResult)_result).StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        }

        private void ThenStatusCode500IsReturned()
        {
            ((StatusCodeResult)_result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }

        private void ThenTheExpectedEnvironmentIsReturned()
        {
            var returnedEnvironment = ((ObjectResult)_result).Value as EnvironmentDetailsDto;
            returnedEnvironment.ShouldBe(_environmentReturnedByFacade);
        }
    }
}
