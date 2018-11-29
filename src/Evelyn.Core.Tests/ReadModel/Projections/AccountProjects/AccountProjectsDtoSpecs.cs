namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects
{
    using System;
    using AutoFixture;
    using Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Xunit;

    public class AccountProjectsDtoSpecs : DtoHarness<AccountProjectsDto>
    {
        [Fact]
        public void Serialization()
        {
            var accountProjects = AccountProjectsDto.Create(
                DataFixture.Create<EventAuditDto>(),
                DataFixture.Create<Guid>());

            accountProjects.AddProject(
                DataFixture.Create<EventAuditDto>(),
                DataFixture.Create<Guid>(),
                DataFixture.Create<string>());

            AssertSerializationOf(accountProjects);
        }
    }
}
