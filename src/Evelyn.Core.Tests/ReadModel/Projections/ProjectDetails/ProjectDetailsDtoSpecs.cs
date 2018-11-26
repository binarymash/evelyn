namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails
{
    using AutoFixture;
    using Core.ReadModel.Projections.ProjectDetails;
    using Xunit;

    public class ProjectDetailsDtoSpecs : DtoHarness<ProjectDetailsDto>
    {
        [Fact]
        public void Serialization()
        {
            var projectDetails = DataFixture.Create<ProjectDetailsDto>();
            AssertSerializationOf(projectDetails);
        }
    }
}
