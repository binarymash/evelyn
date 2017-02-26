namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors
{
    using System;
    using BinaryMash.Responses;
    using Evelyn.Agent.Mediatr;
    using Microsoft.Extensions.Logging;
    using Moq;

    public class LoggingBehaviourSpecs<TRequest, TResponse> : BehaviourSpecs<TRequest, TResponse>
        where TResponse : Response
    {
        public LoggingBehaviourSpecs()
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
