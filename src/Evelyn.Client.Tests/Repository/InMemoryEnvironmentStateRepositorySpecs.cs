namespace Evelyn.Client.Tests.Repository
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Client.Repository;
    using Domain;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class InMemoryEnvironmentStateRepositorySpecs
    {
        private readonly Fixture _fixture;
        private readonly InMemoryEnvironmentStateRepository _repo;

        private List<ToggleState> _togglesInRepo;

        private bool? _retrievedToggle1State;
        private bool? _retrievedToggle2State;
        private bool? _retrievedToggle3State;

        private Exception _thrownException;

        public InMemoryEnvironmentStateRepositorySpecs()
        {
            _fixture = new Fixture();
            _repo = new InMemoryEnvironmentStateRepository();
        }

        [Fact]
        public void SettingRepoEnvironmentStateToNull()
        {
            this.When(_ => WhenWeSetTheEnvironmentStateToNull())
                .Then(_ => ThenAnInvalidEnvironmentStateExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void GettingStateForAnUnknownToggle()
        {
            this.Given(_ => GivenThatThereIsSomeStateInformationInTheRepository())
                .When(_ => WhenWeGetAToggleState())
                .Then(_ => ThenTheDefaultValueIsReturned())
                .And(_ => ThenNoExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void GettingStateForKnownToggles()
        {
            this.Given(_ => GivenThatThereIsSomeStateInformationInTheRepository())
                .When(_ => WhenWeGetTheToggleStates())
                .Then(_ => ThenTheToggleValuesAreReturned())
                .And(_ => ThenNoExceptionIsThrown())
                .BDDfy();
        }

        private void GivenThatThereIsNoStateInformationInTheRepo()
        {
        }

        private void GivenThatThereIsSomeStateInformationInTheRepository()
        {
            _togglesInRepo = new List<ToggleState>();
            for (var i = 0; i < 3; i++)
            {
                _togglesInRepo.Add(new ToggleState(_fixture.Create<string>(), _fixture.Create<bool>()));
            }

            var environmentState = new EnvironmentState(_fixture.Create<int>(), _togglesInRepo);

            _repo.Set(environmentState);
        }

        private void WhenWeGetAToggleState()
        {
            _retrievedToggle1State = WhenWeGetTheToggleStateFor(_fixture.Create<string>());
        }

        private void WhenWeSetTheEnvironmentStateToNull()
        {
            try
            {
                _repo.Set(null);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        private void WhenWeSetTheEnvironmentStateWithNullToggleStates()
        {
            try
            {
                _repo.Set(new EnvironmentState(_fixture.Create<int>(), null));
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        private void WhenWeGetTheToggleStates()
        {
            _retrievedToggle1State = WhenWeGetTheToggleStateFor(_togglesInRepo[0].Key);
            _retrievedToggle2State = WhenWeGetTheToggleStateFor(_togglesInRepo[1].Key);
            _retrievedToggle3State = WhenWeGetTheToggleStateFor(_togglesInRepo[2].Key);
        }

        private bool? WhenWeGetTheToggleStateFor(string toggleKey)
        {
            try
            {
                return _repo.Get(toggleKey);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }

            return null;
        }

        private void ThenAnInvalidEnvironmentStateExceptionIsThrown()
        {
            _thrownException.Should().BeOfType<InvalidEnvironmentStateException>();
        }

        private void ThenNoExceptionIsThrown()
        {
            _thrownException.Should().BeNull();
        }

        private void ThenTheDefaultValueIsReturned()
        {
            ThenTheToggleValueIsReturned(_retrievedToggle1State, default);
        }

        private void ThenTheToggleValuesAreReturned()
        {
            ThenTheToggleValueIsReturned(_retrievedToggle1State, _togglesInRepo[0].Value);
            ThenTheToggleValueIsReturned(_retrievedToggle2State, _togglesInRepo[1].Value);
            ThenTheToggleValueIsReturned(_retrievedToggle3State, _togglesInRepo[2].Value);
        }

        private void ThenTheToggleValueIsReturned(bool? received, bool expected)
        {
            received.HasValue.Should().BeTrue();
            received.Value.Should().Be(expected);
        }
    }
}
