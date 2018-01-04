namespace Evelyn.Core.Tests.ReadModel.ApplicationList
{
    using AutoFixture;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Xunit;

    public class ApplicationListDtoSpecs : DtoSpecs<ApplicationListDto>
    {
        [Fact]
        public void Serialization()
        {
            var applicationList = DataFixture.Create<ApplicationListDto>();

            AssertSerializationOf(applicationList);
        }
    }
}
