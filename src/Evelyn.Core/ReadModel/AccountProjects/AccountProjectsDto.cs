namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AccountProjectsDto
    {
        private readonly List<ProjectListDto> _projects;

        public AccountProjectsDto(Guid accountId)
        {
            AccountId = accountId;
            _projects = new List<ProjectListDto>();
        }

        public Guid AccountId { get; private set; }

        public IEnumerable<ProjectListDto> Projects
        {
            get => _projects.ToList();
            private set => _projects.AddRange(value.ToList());
        }

        public void AddProject(ProjectListDto project)
        {
            _projects.Add(project);
        }
    }
}
