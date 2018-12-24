namespace Evelyn.Core.ReadModel.Projections.AccountProjects.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class Account
    {
        [JsonProperty("projects")]
        private readonly List<Project> _projects;

        [JsonConstructor]
        private Account(Guid accountId, IEnumerable<Project> projects, AggregateAudit audit)
        {
            AccountId = accountId;
            Audit = audit;
            _projects = projects?.ToList() ?? new List<Project>();
        }

        public Guid AccountId { get; private set; }

        public AggregateAudit Audit { get; private set; }

        [JsonIgnore]
        public IEnumerable<Project> Projects => _projects.ToList();

        public static Account Create(EventAudit eventAudit, Guid accountId)
        {
            return new Account(accountId, new List<Project>(), AggregateAudit.Create(eventAudit));
        }

        public void AddProject(EventAudit eventAudit, Guid projectId, string name)
        {
            Audit.Update(eventAudit);

            var project = Project.Create(projectId, name);
            _projects.Add(project);
        }

        public void DeleteProject(EventAudit eventAudit, Guid projectId)
        {
            Audit.Update(eventAudit);

            var project = _projects.Single(p => p.Id == projectId);
            _projects.Remove(project);
        }

        internal void SetProjectName(EventAudit eventAudit, Guid projectId, string name)
        {
            // Don't update audit because the account aggregate hasn't changed
            _projects
                .Single(p => p.Id == projectId)
                .SetName(name);
        }
    }
}
