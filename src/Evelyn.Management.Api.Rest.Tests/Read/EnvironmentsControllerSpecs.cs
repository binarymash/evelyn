namespace Evelyn.Management.Api.Rest.Tests.Read
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Management.Api.Rest.Read;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentsControllerSpecs
    {
        private readonly Fixture _fixture;
        private readonly IReadModelFacade _readModelFacade;
        private readonly EnvironmentsController _controller;
        private EnvironmentDetailsDto _environmentReturnedByFacade;
        private string _keyOfEnvironmentToGet;
        private ObjectResult _result;

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
            _keyOfEnvironmentToGet = _environmentReturnedByFacade.Key;
            _readModelFacade
                .GetEnvironmentDetails(_keyOfEnvironmentToGet)
                .Returns(_environmentReturnedByFacade);
        }

        private void GivenTheEnvironmentWeWantDoesntExist()
        {
            _keyOfEnvironmentToGet = _fixture.Create<string>();
            _readModelFacade
                .GetEnvironmentDetails(_keyOfEnvironmentToGet)
                .Throws(_fixture.Create<NotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingEnvironment()
        {
            _keyOfEnvironmentToGet = _fixture.Create<string>();
            _readModelFacade
                .GetEnvironmentDetails(_keyOfEnvironmentToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetTheEnvironment()
        {
            _result = await _controller.Get(_keyOfEnvironmentToGet);
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

        private void ThenTheExpectedEnvironmentIsReturned()
        {
            var returnedEnvironment = _result.Value as EnvironmentDetailsDto;
            returnedEnvironment.Should().Be(_environmentReturnedByFacade);
        }
    }
}
