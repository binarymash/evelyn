namespace Evelyn.Core.Tests.ReadModel.ToggleDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectionBuilderSpecs : ReadModel.ProjectionBuilderSpecs
    {
        private readonly ProjectionBuilder _builder;
        private readonly Guid _projectId;

        private ToggleDetailsDto _dto;

        private string _toggleKey;

        public ProjectionBuilderSpecs()
        {
            _builder = new ProjectionBuilder(StubbedRepository);
            _projectId = DataFixture.Create<Guid>();
        }

        [Fact]
        public void Toggle1DoesNotExist()
        {
            this.Given(_ => GivenThatWeDontCreateToggle1())
                .When(_ => WhenWeInvokeTheProjectionBuilderForToggle1())
                .Then(_ => ThenAFailedToBuildProjectionExceptionIsThrown())
                .BDDfy();
        }

        private void GivenThatWeDontCreateToggle1()
        {
            _toggleKey = DataFixture.Create<string>();
        }

        private async Task WhenWeInvokeTheProjectionBuilderForToggle1()
        {
            await WhenWeInvokeTheProjectionBuilderFor(new ProjectionBuilderRequest(_projectId, _toggleKey));
        }

        private async Task WhenWeInvokeTheProjectionBuilderFor(ProjectionBuilderRequest request)
        {
            try
            {
                _dto = await _builder.Invoke(request);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenAFailedToBuildProjectionExceptionIsThrown()
        {
            ThrownException.Should().BeOfType<FailedToBuildProjectionException>();
        }
    }
}
