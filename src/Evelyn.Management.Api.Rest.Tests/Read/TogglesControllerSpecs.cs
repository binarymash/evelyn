﻿namespace Evelyn.Management.Api.Rest.Tests.Read
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.ToggleDetails;
    using Evelyn.Core.ReadModel;
    using Evelyn.Management.Api.Rest.Read;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;

    public class TogglesControllerSpecs
    {
        private readonly Fixture _fixture;
        private readonly IReadModelFacade _readModelFacade;
        private readonly TogglesController _controller;
        private ToggleDetailsDto _toggleReturnedByFacade;
        private Guid _idOfToggleToGet;
        private ObjectResult _result;

        public TogglesControllerSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new TogglesController(_readModelFacade);
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
            _toggleReturnedByFacade = _fixture.Create<ToggleDetailsDto>();
            _idOfToggleToGet = _toggleReturnedByFacade.Id;
            _readModelFacade
                .GetToggleDetails(_idOfToggleToGet)
                .Returns(_toggleReturnedByFacade);
        }

        private void GivenTheToggleWeWantDoesntExist()
        {
            _idOfToggleToGet = _fixture.Create<Guid>();
            _readModelFacade
                .GetToggleDetails(_idOfToggleToGet)
                .Throws(_fixture.Create<NotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingToggle()
        {
            _idOfToggleToGet = _fixture.Create<Guid>();
            _readModelFacade
                .GetToggleDetails(_idOfToggleToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetTheToggle()
        {
            _result = await _controller.Get(_idOfToggleToGet);
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
            var returnedToggle = _result.Value as ToggleDetailsDto;
            returnedToggle.Should().Be(_toggleReturnedByFacade);
        }
    }
}