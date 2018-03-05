namespace Evelyn.Core.Tests.ReadModel.ProjectList
{
    using AutoFixture;
    using Core.ReadModel.ProjectList;
    using Xunit;

    public class ProjectListDtoSpecs : DtoSpecs<ProjectListDto>
    {
        [Fact]
        public void Serialization()
        {
            var projectList = DataFixture.Create<ProjectListDto>();

            AssertSerializationOf(projectList);
        }
    }
}
