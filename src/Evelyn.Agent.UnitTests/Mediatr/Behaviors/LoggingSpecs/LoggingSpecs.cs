namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.LoggingSpecs
{
    using System;
    using BinaryMash.Responses;
    using Evelyn.Agent.Mediatr.Behaviors;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Moq;

    public class LoggingSpecs<TRequest, TResponse> : BehaviourSpecs<TRequest, TResponse>
        where TRequest : IRequest
        where TResponse : Response
    {
        public LoggingSpecs()
        {
            Logger = new Mock<ILogger<Logging<TRequest, TResponse>>>();
            Behavior = new Logging<TRequest, TResponse>(Logger.Object);
        }

        protected Mock<ILogger<Logging<TRequest, TResponse>>> Logger { get; set; }

        protected void ThenInformationIsLogged(string expectedValue)
        {
            Logger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<object>(o => o.ToString() == expectedValue),
                    null,
                    It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        protected void ThenInformationIsNotLogged(string expectedValue)
        {
            Logger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<object>(o => o.ToString() == expectedValue),
                    null,
                    It.IsAny<Func<object, Exception, string>>()), Times.Never);
        }
    }
}
