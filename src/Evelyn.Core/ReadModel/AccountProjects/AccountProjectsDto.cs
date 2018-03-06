namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using ProjectList;

    public class AccountProjectsDto
    {
        public AccountProjectsDto(string accountId)
        {
            AccountId = accountId;
            Projects = new Dictionary<Guid, ProjectListDto>();
        }

        public string AccountId { get; private set; }

        public Dictionary<Guid, ProjectListDto> Projects { get; private set; }
    }
}
