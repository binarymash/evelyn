namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class AccountProjectsDto : DtoRoot
    {
        [JsonProperty("projects")]
        private readonly List<ProjectListDto> _projects;

        [JsonConstructor]
        private AccountProjectsDto(Guid accountId, IEnumerable<ProjectListDto> projects, AuditDto audit)
            : base(audit)
        {
            AccountId = accountId;
            _projects = projects?.ToList() ?? new List<ProjectListDto>();
        }

        public Guid AccountId { get; private set; }

        [JsonIgnore]
        public IEnumerable<ProjectListDto> Projects => _projects.ToList();

        public static string StoreKey(Guid accountId)
        {
            return $"{nameof(AccountProjectsDto)}-{accountId}";
        }

        public static AccountProjectsDto Create(Guid accountId, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            return new AccountProjectsDto(accountId, new List<ProjectListDto>(), AuditDto.Create(occurredAt, initiatedBy, newVersion));
        }

        public void AddProject(Guid projectId, string name, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);

            var project = new ProjectListDto(projectId, name);
            _projects.Add(project);
        }

        public void DeleteProject(Guid projectId, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);

            var project = _projects.Single(p => p.Id == projectId);
            _projects.Remove(project);
        }

        internal void SetProjectName(Guid projectId, string name, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);

            _projects
                .Single(p => p.Id == projectId)
                .SetName(name);
        }
    }
}
