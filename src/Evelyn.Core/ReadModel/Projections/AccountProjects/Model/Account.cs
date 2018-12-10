namespace Evelyn.Core.ReadModel.Projections.AccountProjects.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Account
    {
        [JsonProperty("projects")]
        private readonly List<Project> _projects;

        [JsonConstructor]
        private Account(Guid accountId, IEnumerable<Project> projects, AuditDto audit)
        {
            AccountId = accountId;
            Audit = audit;
            _projects = projects?.ToList() ?? new List<Project>();
        }

        public Guid AccountId { get; private set; }

        public AuditDto Audit { get; private set; }

        [JsonIgnore]
        public IEnumerable<Project> Projects => _projects.ToList();

        public static Account Create(EventAuditDto eventAudit, Guid accountId)
        {
            return new Account(accountId, new List<Project>(), AuditDto.Create(eventAudit));
        }

        public void AddProject(EventAuditDto eventAudit, Guid projectId, string name)
        {
            Audit.Update(eventAudit);

            var project = Project.Create(projectId, name);
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
            // Don't update audit because the account aggregate hasn't changed
            _projects
                .Single(p => p.Id == projectId)
                .SetName(name);
        }
    }
}
