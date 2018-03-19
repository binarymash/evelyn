namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AccountProjectsDto
    {
        private readonly List<ProjectListDto> _projects;

        public AccountProjectsDto(Guid accountId, DateTimeOffset created, DateTimeOffset lastModified, IEnumerable<ProjectListDto> projects)
        {
            AccountId = accountId;
            Created = created;
            LastModified = lastModified;
            _projects = projects?.ToList() ?? new List<ProjectListDto>();
        }

        public Guid AccountId { get; private set; }

        public DateTimeOffset Created { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public IEnumerable<ProjectListDto> Projects => _projects.ToList();
    }
}
