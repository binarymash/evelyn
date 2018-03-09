namespace Evelyn.Core.Tests.ReadModel.ToggleDetails
{
    using System;
    using AutoFixture;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleDetailsHandlerSpecs : HandlerSpecs
    {
        private string _toggleKey;

        [Fact]
        public void Toggle1DoesNotExist()
        {
            this.Given(_ => GivenThatWeDontCreateToggle1())
                .When(_ => WhenWeGetToggle1Details())
                .Then(_ => ThenGettingToggle1ThrownsNotFoundException())
                .BDDfy();
        }

        protected override void RegisterHandlers(Router router)
        {
            var handler = new ToggleDetailsHandler(ToggleDetailsStore);
            router.RegisterHandler<ToggleAdded>(handler.Handle);
        }

        private void GivenThatWeDontCreateToggle1()
        {
            _toggleKey = DataFixture.Create<string>();
        }

        private void WhenWeGetToggle1Details()
        {
            try
            {
                ReadModelFacade.GetToggleDetails(_toggleKey).GetAwaiter().GetResult().Should().BeNull();
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenGettingToggle1ThrownsNotFoundException()
        {
            ThrownException.Should().BeOfType<NotFoundException>();
        }
    }
}
