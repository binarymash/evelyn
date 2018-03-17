namespace Evelyn.Core.Tests.ReadModel.ProjectDetails
{
    using AutoFixture;
    using Core.ReadModel.ProjectDetails;
    using Xunit;

    public class ProjectDetailsDtoSpecs : DtoSpecs<ProjectDetailsDto>
    {
        [Fact]
        public void Serialization()
        {
            var projectDetails = DataFixture.Create<ProjectDetailsDto>();
            AssertSerializationOf(projectDetails);
        }
    }
}
