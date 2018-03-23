namespace Evelyn.Client.Tests
{
    using AutoFixture;
    using Client.Repository;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class EvelynClientSpecs
    {
        private readonly Fixture _fixture;
        private readonly EvelynClient _evelynClient;
        private readonly IEnvironmentStateRepository _repo;
        private readonly string _toggleKey;

        private bool _expectedState;
        private bool _result;

        public EvelynClientSpecs()
        {
            _fixture = new Fixture();
            _repo = Substitute.For<IEnvironmentStateRepository>();
            _evelynClient = new EvelynClient(_repo);
            _toggleKey = _fixture.Create<string>();
        }

        [Fact]
        public void GetsToggleState()
        {
            this.Given(_ => GivenTheStateRepoWillReturnAToggleState())
                .When(_ => WhenWeGetAToggleState())
                .Then(_ => ThenTheToggleStateIsRetrievedFromTheStateRepository())
                .And(_ => ThenTheToggleStateIsReturned())
                .BDDfy();
        }

        private void GivenTheStateRepoWillReturnAToggleState()
        {
            _expectedState = _fixture.Create<bool>();
            _repo.Get(Arg.Any<string>()).ReturnsForAnyArgs(_expectedState);
        }

        private void WhenWeGetAToggleState()
        {
           _result = _evelynClient.GetToggleState(_toggleKey);
        }

        private void ThenTheToggleStateIsRetrievedFromTheStateRepository()
        {
            _repo.Received().Get(_toggleKey);
        }

        private void ThenTheToggleStateIsReturned()
        {
            _result.Should().Be(_expectedState);
        }
    }
}
