namespace Evelyn.Management.Api.Rest.Tests.Read
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel;
    using Core.ReadModel.Projections.ClientEnvironmentState;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Rest.Read;
    using TestStack.BDDfy;
    using Xunit;

    public class ClientEnvironmentStatesControllerSpecs
    {
        private readonly Fixture _fixture;
        private readonly IReadModelFacade _readModelFacade;
        private readonly ClientEnvironmentStatesController _controller;
        private readonly Guid _projectId;

        private Projection _clientEnvironmentStateReturnedByFacade;
        private string _keyOfEnvironmentStateToGet;
        private ObjectResult _result;

        public ClientEnvironmentStatesControllerSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new ClientEnvironmentStatesController(_readModelFacade);
            _projectId = _fixture.Create<Guid>();
        }

        [Fact]
        public void GetsEnvironmentState()
        {
            this.Given(_ => GivenTheEnvironmentStateWeWantDoesExist())
                .When(_ => WhenWeGetTheEnvironmentState())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenTheExpectedEnvironmentStateIsReturned())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentNotFound()
        {
            this.Given(_ => GivenTheEnvironmentStateWeWantDoesntExist())
                .When(_ => WhenWeGetTheEnvironmentState())
                .Then(_ => ThenStatusCode404IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingEnvironment()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownWhenGettingEnvironmentState())
                .When(_ => WhenWeGetTheEnvironmentState())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        private void GivenTheEnvironmentStateWeWantDoesExist()
        {
            _clientEnvironmentStateReturnedByFacade = _fixture.Create<Projection>();
            _keyOfEnvironmentStateToGet = _fixture.Create<string>();

            _readModelFacade
                .GetClientEnvironmentState(_projectId, _keyOfEnvironmentStateToGet)
                .Returns(_clientEnvironmentStateReturnedByFacade);
        }

        private void GivenTheEnvironmentStateWeWantDoesntExist()
        {
            _keyOfEnvironmentStateToGet = _fixture.Create<string>();

            _readModelFacade
                .GetClientEnvironmentState(_projectId, _keyOfEnvironmentStateToGet)
                .Throws(_fixture.Create<ProjectionNotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingEnvironmentState()
        {
            _keyOfEnvironmentStateToGet = _fixture.Create<string>();

            _readModelFacade
                .GetClientEnvironmentState(_projectId, _keyOfEnvironmentStateToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetTheEnvironmentState()
        {
            _result = await _controller.Get(_projectId, _keyOfEnvironmentStateToGet);
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

        private void ThenTheExpectedEnvironmentStateIsReturned()
        {
            var returnedEnvironmentState = _result.Value as Projection;
            returnedEnvironmentState.Should().Be(_clientEnvironmentStateReturnedByFacade);
        }
    }
}
