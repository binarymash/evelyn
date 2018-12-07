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
        private AccountProjectsDto(Guid accountId, IEnumerable<ProjectListDto> projects, ProjectionAuditDto audit, AuditDto accountAudit)
            : base(audit)
        {
            AccountId = accountId;
            AccountAudit = accountAudit;
            _projects = projects?.ToList() ?? new List<ProjectListDto>();
        }

        public Guid AccountId { get; private set; }

        public AuditDto AccountAudit { get; private set; }

        [JsonIgnore]
        public IEnumerable<ProjectListDto> Projects => _projects.ToList();

        public static string StoreKey(Guid accountId)
        {
            return $"{nameof(AccountProjectsDto)}-{accountId}";
        }

        public static AccountProjectsDto Create(EventAuditDto eventAudit, Guid accountId)
        {
            return new AccountProjectsDto(accountId, new List<ProjectListDto>(), ProjectionAuditDto.Create(eventAudit), AuditDto.Create(eventAudit));
        }

        public void AddProject(EventAuditDto eventAudit, Guid projectId, string name)
        {
            Audit.Update(eventAudit);
            AccountAudit.Update(eventAudit);

            var project = new ProjectListDto(projectId, name);
            _projects.Add(project);
        }

        public void DeleteProject(EventAuditDto eventAudit, Guid projectId)
        {
            Audit.Update(eventAudit);
            AccountAudit.Update(eventAudit);

            var project = _projects.Single(p => p.Id == projectId);
            _projects.Remove(project);
        }

        internal void SetProjectName(EventAuditDto eventAudit, Guid projectId, string name)
        {
            // Don't update account because the account hasn't changed
            Audit.Update(eventAudit);

            _projects
                .Single(p => p.Id == projectId)
                .SetName(name);
        }
    }
}
