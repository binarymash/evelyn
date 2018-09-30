namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects
{
    using System;
    using AutoFixture;
    using Core.ReadModel.Projections.AccountProjects;
    using Xunit;

    public class AccountProjectsDtoSpecs : DtoSpecs<AccountProjectsDto>
    {
        [Fact]
        public void Serialization()
        {
            var accountProjects = AccountProjectsDto.Create(
                DataFixture.Create<Guid>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>());

            accountProjects.AddProject(
                DataFixture.Create<Guid>(),
                DataFixture.Create<string>(),
                DataFixture.Create<int>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>());

            AssertSerializationOf(accountProjects);
        }
    }
}
