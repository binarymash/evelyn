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
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.Core;
    using Provider;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentStateSynchronizerSpecs
    {
        private readonly Fixture _fixture;
        private readonly EnvironmentStateSynchronizer _synchronizer;
        private readonly IEnvironmentStateProvider _provider;
        private readonly IEnvironmentStateRepository _repo;
        private readonly IOptions<EnvironmentStateSynchronizerOptions> _options;
        private readonly IList<EnvironmentState> _expectedEnvironmentStates;
        private readonly IList<EnvironmentState> _storedEnvironmentStates;

        public EnvironmentStateSynchronizerSpecs()
        {
            _fixture = new Fixture();
            _provider = Substitute.For<IEnvironmentStateProvider>();
            _repo = Substitute.For<IEnvironmentStateRepository>();
            _expectedEnvironmentStates = new List<EnvironmentState>();
            _storedEnvironmentStates = new List<EnvironmentState>();

            _options = Options.Create(_fixture
                .Build<EnvironmentStateSynchronizerOptions>()
                .With(o => o.PollingPeriod, TimeSpan.FromSeconds(1))
                .Create());

            _synchronizer = new EnvironmentStateSynchronizer(_provider, _repo, _options);

            _repo.WhenForAnyArgs(repo => repo.Set(Arg.Any<EnvironmentState>()))
                .Do(StoreCallInfo);
        }

        [Fact]
        public void SynchronizingEnvironmentStates()
        {
            this.Given(_ => GivenTheEnvironmentStateProviderReturnsSomeStates())
                .When(_ => WhenWeStartTheSynchronizer())
                .And(_ => WhenWeWaitAFewSecondsAndThenStopTheSynchronizer())
                .And(_ => ThenTheSynchronizerStoresTheReturnedEnvironmentStatesInTheRepo())
                .BDDfy();
        }

        private void StoreCallInfo(CallInfo callInfo)
        {
            _storedEnvironmentStates.Add(callInfo.ArgAt<EnvironmentState>(0));
        }

        private void GivenTheEnvironmentStateProviderReturnsSomeStates()
        {
            for (var i = 0; i < 3; i++)
            {
                _expectedEnvironmentStates.Add(new EnvironmentState(_fixture.Create<int>(), _fixture.CreateMany<ToggleState>()));
            }

            _provider.Invoke(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(_expectedEnvironmentStates[0], _expectedEnvironmentStates[1], _expectedEnvironmentStates[2]);
        }

        private async Task WhenWeStartTheSynchronizer()
        {
            await _synchronizer.StartAsync(default);
        }

        private async Task WhenWeWaitAFewSecondsAndThenStopTheSynchronizer()
        {
            while (_storedEnvironmentStates.Count < 3)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            await _synchronizer.StopAsync(default);
        }

        private void ThenTheSynchronizerStoresTheReturnedEnvironmentStatesInTheRepo()
        {
            _storedEnvironmentStates.Should().BeEquivalentTo(_expectedEnvironmentStates);
        }
    }
}