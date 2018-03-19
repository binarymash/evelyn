namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AccountProjectsDto : DtoRoot
    {
        private readonly List<ProjectListDto> _projects;

        public AccountProjectsDto(Guid accountId, DateTimeOffset created, DateTimeOffset lastModified, IEnumerable<ProjectListDto> projects)
            : base(created, lastModified)
        {
            AccountId = accountId;
            _projects = projects?.ToList() ?? new List<ProjectListDto>();
        }

        public Guid AccountId { get; private set; }

        public IEnumerable<ProjectListDto> Projects => _projects.ToList();
    }
}
