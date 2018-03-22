namespace Evelyn.Core.Tests.ReadModel.AccountProjects
{
    using AutoFixture;
    using Core.ReadModel.AccountProjects;
    using Xunit;

    public class AccountProjectsDtoSpecs : DtoSpecs<AccountProjectsDto>
    {
        [Fact]
        public void Serialization()
        {
            var accountProjects = DataFixture.Create<AccountProjectsDto>();

            AssertSerializationOf(accountProjects);
        }
    }
}
