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

        public static AccountProjectsDto Create(EventAuditDto eventAudit, Guid accountId)
        {
            return new AccountProjectsDto(accountId, new List<ProjectListDto>(), AuditDto.Create(eventAudit));
        }

        public void AddProject(EventAuditDto eventAudit, Guid projectId, string name)
        {
            Audit.Update(eventAudit);

            var project = new ProjectListDto(projectId, name);
            _projects.Add(project);
        }

        public void DeleteProject(EventAuditDto eventAudit, Guid projectId)
        {
            Audit.Update(eventAudit);

            var project = _projects.Single(p => p.Id == projectId);
            _projects.Remove(project);
        }

        internal void SetProjectName(EventAuditDto eventAudit, Guid projectId, string name)
        {
            Audit.Update(eventAudit);

            _projects
                .Single(p => p.Id == projectId)
                .SetName(name);
        }
    }
}
