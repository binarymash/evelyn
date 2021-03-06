namespace Evelyn.Management.Api.Rest.Tests.Read.EnvironmentsController
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
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
        private readonly EnvironmentsController _controller;
        private readonly Guid _projectId;

        private Projection _environmentProjectionReturnedByFacade;
        private string _keyOfEnvironmentToGet;
        private ObjectResult _result;

        public GetDefinitionSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new EnvironmentsController(_readModelFacade);
            _projectId = _fixture.Create<Guid>();
        }

        [Fact]
        public void GetsEnvironment()
        {
            this.Given(_ => GivenTheEnvironmentWeWantDoesExist())
                .When(_ => WhenWeGetTheEnvironmentDefinition())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenTheExpectedEnvironmentIsReturned())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentNotFound()
        {
            this.Given(_ => GivenTheEnvironmentWeWantDoesntExist())
                .When(_ => WhenWeGetTheEnvironmentDefinition())
                .Then(_ => ThenStatusCode404IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingEnvironment()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownWhenGettingEnvironment())
                .When(_ => WhenWeGetTheEnvironmentDefinition())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        private void GivenTheEnvironmentWeWantDoesExist()
        {
            _environmentProjectionReturnedByFacade = _fixture.Create<Projection>();
            _keyOfEnvironmentToGet = _environmentProjectionReturnedByFacade.Environment.Key;
            _readModelFacade
                .GetEnvironmentDetails(_projectId, _keyOfEnvironmentToGet)
                .Returns(_environmentProjectionReturnedByFacade);
        }

        private void GivenTheEnvironmentWeWantDoesntExist()
        {
            _keyOfEnvironmentToGet = _fixture.Create<string>();
            _readModelFacade
                .GetEnvironmentDetails(_projectId, _keyOfEnvironmentToGet)
                .Throws(_fixture.Create<ProjectionNotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingEnvironment()
        {
            _keyOfEnvironmentToGet = _fixture.Create<string>();
            _readModelFacade
                .GetEnvironmentDetails(_projectId, _keyOfEnvironmentToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetTheEnvironmentDefinition()
        {
            _result = await _controller.GetDefinition(_projectId, _keyOfEnvironmentToGet);
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
            var returnedEnvironment = _result.Value as Projection;
            returnedEnvironment.Should().Be(_environmentProjectionReturnedByFacade);
        }
    }
}
