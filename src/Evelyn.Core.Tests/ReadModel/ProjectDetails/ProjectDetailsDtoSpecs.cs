namespace Evelyn.Core.Tests.ReadModel.ProjectDetails
{
    using System;
    using AutoFixture;
    using Core.ReadModel.ProjectDetails;
    using Xunit;

    public class ProjectDetailsDtoSpecs : DtoSpecs<ProjectDetailsDto>
    {
        [Fact]
        public void Serialization()
        {
            // TODO: inspect props are set
            var projectDetails = DataFixture.Create<ProjectDetailsDto>();
            ////projectDetails.AddEnvironment(DataFixture.Create<EnvironmentListDto>(), DataFixture.Create<DateTimeOffset>(), DataFixture.Create<int>());
            ////projectDetails.AddEnvironment(DataFixture.Create<EnvironmentListDto>(), DataFixture.Create<DateTimeOffset>(), DataFixture.Create<int>());
            ////projectDetails.AddToggle(DataFixture.Create<ToggleListDto>(), DataFixture.Create<DateTimeOffset>(), DataFixture.Create<int>());
            ////projectDetails.AddToggle(DataFixture.Create<ToggleListDto>(), DataFixture.Create<DateTimeOffset>(), DataFixture.Create<int>());

            AssertSerializationOf(projectDetails);
        }
    }
}
