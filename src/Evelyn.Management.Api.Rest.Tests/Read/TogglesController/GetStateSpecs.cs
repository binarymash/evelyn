namespace Evelyn.Management.Api.Rest.Tests.Read.TogglesController
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel;
    using Core.ReadModel.Projections.ToggleState;
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
        private readonly TogglesController _controller;
        private readonly Guid _projectId;

        private Projection _toggleStateReturnedByFacade;
        private string _keyOfToggleStateToGet;
        private ObjectResult _result;

        public GetStateSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new TogglesController(_readModelFacade);
            _projectId = _fixture.Create<Guid>();
        }

        [Fact]
        public void GetsToggleState()
        {
            this.Given(_ => GivenTheToggleStateWeWantDoesExist())
                .When(_ => WhenWeGetTheToggleState())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenTheExpectedToggleStateIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ToggleNotFound()
        {
            this.Given(_ => GivenTheToggleStateWeWantDoesntExist())
                .When(_ => WhenWeGetTheToggleState())
                .Then(_ => ThenStatusCode404IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingToggleState()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownWhenGettingToggleState())
                .When(_ => WhenWeGetTheToggleState())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        private void GivenTheToggleStateWeWantDoesExist()
        {
            _toggleStateReturnedByFacade = _fixture.Create<Projection>();
            _keyOfToggleStateToGet = _fixture.Create<string>();

            _readModelFacade.GetToggleState(_projectId, _keyOfToggleStateToGet)
                .Returns(_toggleStateReturnedByFacade);
        }

        private void GivenTheToggleStateWeWantDoesntExist()
        {
            _keyOfToggleStateToGet = _fixture.Create<string>();

            _readModelFacade
                .GetToggleState(_projectId, _keyOfToggleStateToGet)
                .Throws(_fixture.Create<ProjectionNotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingToggleState()
        {
            _keyOfToggleStateToGet = _fixture.Create<string>();

            _readModelFacade
                .GetToggleState(_projectId, _keyOfToggleStateToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetTheToggleState()
        {
            _result = await _controller.GetState(_projectId, _keyOfToggleStateToGet);
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

        private void ThenTheExpectedToggleStateIsReturned()
        {
            var returnedToggleState = _result.Value as Projection;
            returnedToggleState.Should().Be(_toggleStateReturnedByFacade);
        }
    }
}
