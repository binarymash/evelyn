﻿namespace Evelyn.Core.ReadModel.Projections.ProjectDetails.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Project
    {
        private readonly IList<Environment> _environments;
        private readonly IList<Toggle> _toggles;

        [JsonConstructor]
        private Project(Guid id, string name, IEnumerable<Environment> environments, IEnumerable<Toggle> toggles, AuditDto audit)
        {
            Id = id;
            Name = name;
            Audit = audit;
            _environments = environments?.ToList() ?? new List<Environment>();
            _toggles = toggles?.ToList() ?? new List<Toggle>();
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public AuditDto Audit { get; private set; }

        public IEnumerable<Environment> Environments => _environments;

        public IEnumerable<Toggle> Toggles => _toggles;

        public static Project Create(EventAuditDto eventAudit, Guid id, string name)
        {
            return new Project(id, name, new List<Environment>(), new List<Toggle>(), AuditDto.Create(eventAudit));
        }

        public void AddEnvironment(EventAuditDto eventAudit, string environmentKey, string environmentName)
        {
            Audit.Update(eventAudit);
            Audit.Update(eventAudit);

            var environment = Environment.Create(environmentKey, environmentName);
            _environments.Add(environment);
        }

        public void DeleteEnvironment(EventAuditDto eventAudit, string environmentKey)
        {
            Audit.Update(eventAudit);
            Audit.Update(eventAudit);

            var environment = _environments.Single(e => e.Key == environmentKey);
            _environments.Remove(environment);
        }

        public void AddToggle(EventAuditDto eventAudit, string toggleKey, string toggleName)
        {
            Audit.Update(eventAudit);
            Audit.Update(eventAudit);

            var toggleToAdd = Toggle.Create(toggleKey, toggleName);
            _toggles.Add(toggleToAdd);
        }

        public void DeleteToggle(EventAuditDto eventAudit, string toggleKey)
        {
            Audit.Update(eventAudit);
            Audit.Update(eventAudit);

            var toggleToRemove = _toggles.Single(toggle => toggle.Key == toggleKey);
            _toggles.Remove(toggleToRemove);
        }
    }
}
