namespace Evelyn.Management.Api.Rest.Tests.Read.TogglesController
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.Projections.ToggleDetails;
    using Evelyn.Core.ReadModel;
    using Evelyn.Management.Api.Rest.Read;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;

    public class GetDefinitionSpecs
    {
        private readonly Fixture _fixture;
        private readonly IReadModelFacade _readModelFacade;
        private readonly TogglesController _controller;
        private Guid _projectId;
        private Projection _projectionReturnedByFacade;
        private string _keyOfToggleToGet;
        private ObjectResult _result;

        public GetDefinitionSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new TogglesController(_readModelFacade);
            _projectId = _fixture.Create<Guid>();
        }

        [Fact]
        public void GetsToggle()
        {
            this.Given(_ => GivenTheToggleWeWantDoesExist())
                .When(_ => WhenWeGetTheToggle())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenTheExpectedToggleIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ToggleNotFound()
        {
            this.Given(_ => GivenTheToggleWeWantDoesntExist())
                .When(_ => WhenWeGetTheToggle())
                .Then(_ => ThenStatusCode404IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingToggle()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownWhenGettingToggle())
                .When(_ => WhenWeGetTheToggle())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        private void GivenTheToggleWeWantDoesExist()
        {
            _projectionReturnedByFacade = _fixture.Create<Projection>();
            _keyOfToggleToGet = _projectionReturnedByFacade.Toggle.Key;
            _readModelFacade
                .GetToggleDetails(_projectId, _keyOfToggleToGet)
                .Returns(_projectionReturnedByFacade);
        }

        private void GivenTheToggleWeWantDoesntExist()
        {
            _keyOfToggleToGet = _fixture.Create<string>();
            _readModelFacade
                .GetToggleDetails(_projectId, _keyOfToggleToGet)
                .Throws(_fixture.Create<ProjectionNotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingToggle()
        {
            _keyOfToggleToGet = _fixture.Create<string>();
            _readModelFacade
                .GetToggleDetails(_projectId, _keyOfToggleToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetTheToggle()
        {
            _result = await _controller.GetDefinition(_projectId, _keyOfToggleToGet);
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

        private void ThenTheExpectedToggleIsReturned()
        {
            var returnedToggle = _result.Value as Projection;
            returnedToggle.Should().Be(_projectionReturnedByFacade);
        }
    }
}
