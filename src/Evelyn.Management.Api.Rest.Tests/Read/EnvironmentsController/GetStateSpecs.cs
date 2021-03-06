namespace Evelyn.Management.Api.Rest.Tests.Read.EnvironmentsController
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel;
    using Core.ReadModel.Projections.EnvironmentState;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Rest.Read;
    using TestStack.BDDfy;
    using Xunit;

    public class GetStateSpecs
    {
        private readonly Fixture _fixture;
        private readonly IReadModelFacade _readModelFacade;
        private readonly EnvironmentsController _controller;
        private readonly Guid _projectId;

        private Projection _environmentStateReturnedByFacade;
        private string _keyOfEnvironmentStateToGet;
        private ObjectResult _result;

        public GetStateSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new EnvironmentsController(_readModelFacade);
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
            _environmentStateReturnedByFacade = _fixture.Create<Projection>();
            _keyOfEnvironmentStateToGet = _fixture.Create<string>();

            _readModelFacade
                .GetEnvironmentState(_projectId, _keyOfEnvironmentStateToGet)
                .Returns(_environmentStateReturnedByFacade);
        }

        private void GivenTheEnvironmentStateWeWantDoesntExist()
        {
            _keyOfEnvironmentStateToGet = _fixture.Create<string>();

            _readModelFacade
                .GetEnvironmentState(_projectId, _keyOfEnvironmentStateToGet)
                .Throws(_fixture.Create<ProjectionNotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingEnvironmentState()
        {
            _keyOfEnvironmentStateToGet = _fixture.Create<string>();

            _readModelFacade
                .GetEnvironmentState(_projectId, _keyOfEnvironmentStateToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetTheEnvironmentState()
        {
            _result = await _controller.GetState(_projectId, _keyOfEnvironmentStateToGet);
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
            returnedEnvironmentState.Should().Be(_environmentStateReturnedByFacade);
        }
    }
}
