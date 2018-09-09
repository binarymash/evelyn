namespace Evelyn.Client.Tests.Synchronization
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Client.Repository;
    using Client.Synchronization;
    using Domain;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.Core;
    using NSubstitute.ExceptionExtensions;
    using Provider;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentStateSynchronizerSpecs
    {
        private readonly Fixture _fixture;
        private readonly EnvironmentStatePollingSynchronizer _synchronizer;
        private readonly IEnvironmentStateProvider _provider;
        private readonly IEnvironmentStateRepository _repo;
        private readonly ILogger<EnvironmentStatePollingSynchronizer> _logger;
        private readonly IOptions<EnvironmentStatePollingSynchronizerOptions> _pollingOptions;
        private readonly IOptions<EnvironmentOptions> _environmentOptions;
        private readonly IList<EnvironmentState> _expectedEnvironmentStates;
        private readonly IList<EnvironmentState> _storedEnvironmentStates;

        public EnvironmentStateSynchronizerSpecs()
        {
            _fixture = new Fixture();
            _provider = Substitute.For<IEnvironmentStateProvider>();
            _repo = Substitute.For<IEnvironmentStateRepository>();
            _logger = Substitute.For<ILogger<EnvironmentStatePollingSynchronizer>>();
            _expectedEnvironmentStates = new List<EnvironmentState>();
            _storedEnvironmentStates = new List<EnvironmentState>();

            _pollingOptions = Options.Create(
                new EnvironmentStatePollingSynchronizerOptions
                {
                    PollingPeriod = TimeSpan.FromSeconds(1)
                });

            _environmentOptions = Options.Create(_fixture.Create<EnvironmentOptions>());

            _synchronizer = new EnvironmentStatePollingSynchronizer(_logger, _provider, _repo, _pollingOptions, _environmentOptions);

            _repo.WhenForAnyArgs(repo => repo.Set(Arg.Any<EnvironmentState>()))
                .Do(StoreCallInfo);
        }

        [Fact]
        public void SynchronizingEnvironmentStates()
        {
            this.Given(_ => GivenTheProviderWillReturnsSomeStates())
                .When(_ => WhenWeStartTheSynchronizer())
                .And(_ => WhenWeWaitAFewSecondsAndThenStopTheSynchronizer())
                .And(_ => ThenTheSynchronizerStoresTheReturnedEnvironmentStatesInTheRepo())
                .BDDfy();
        }

        [Fact]
        public void SynchronizationExceptionFromProvider()
        {
            this.Given(_ => GivenTheProviderThrowsASynchronizationException())
                .When(_ => WhenWeStartTheSynchronizer())
                .And(_ => WhenWeWaitAFewSecondsAndThenStopTheSynchronizer())
                .Then(_ => ThenNothingIsSavedToTheRepo())
                .BDDfy();
        }

        private void StoreCallInfo(CallInfo callInfo)
        {
            _storedEnvironmentStates.Add(callInfo.ArgAt<EnvironmentState>(0));
        }

        private void GivenTheProviderWillReturnsSomeStates()
        {
            for (var i = 0; i < 3; i++)
            {
                _expectedEnvironmentStates.Add(new EnvironmentState(_fixture.Create<int>(), _fixture.CreateMany<ToggleState>()));
            }

            _provider.Invoke(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(_expectedEnvironmentStates[0], _expectedEnvironmentStates[1], _expectedEnvironmentStates[2]);
        }

        private void GivenTheProviderThrowsASynchronizationException()
        {
            _provider
                .Invoke(Arg.Any<Guid>(), Arg.Any<string>())
                .ThrowsForAnyArgs(_fixture.Create<SynchronizationException>());
        }

        private async Task WhenWeStartTheSynchronizer()
        {
            await _synchronizer.StartAsync(default);
        }

        private async Task WhenWeWaitAFewSecondsAndThenStopTheSynchronizer()
        {
            var timeToStopWaiting = DateTime.UtcNow.AddSeconds(4);
            while (_storedEnvironmentStates.Count < 3 && DateTime.UtcNow < timeToStopWaiting)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            await _synchronizer.StopAsync(default);
        }

        private void ThenTheSynchronizerStoresTheReturnedEnvironmentStatesInTheRepo()
        {
            _storedEnvironmentStates.Should().BeEquivalentTo(_expectedEnvironmentStates);
        }

        private void ThenNothingIsSavedToTheRepo()
        {
            _storedEnvironmentStates.Count.Should().Be(0);
        }
    }
}