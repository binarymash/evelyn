namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProjectList;

    public class AccountProjectsDto
    {
        private readonly List<ProjectListDto> _projects;

        public AccountProjectsDto(Guid accountId)
        {
            AccountId = accountId;
            _projects = new List<ProjectListDto>();
        }

        public Guid AccountId { get; private set; }

        public IEnumerable<ProjectListDto> Projects => _projects.ToList();

        public void AddProject(ProjectListDto project)
        {
            _projects.Add(project);
        }
    }
}
