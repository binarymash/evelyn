namespace Evelyn.Core.ReadModel.Projections.ProjectDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class ProjectDetailsDto : DtoRoot
    {
        private readonly IList<EnvironmentListDto> _environments;
        private readonly IList<ToggleListDto> _toggles;

        [JsonConstructor]
        private ProjectDetailsDto(Guid id, string name, IEnumerable<EnvironmentListDto> environments, IEnumerable<ToggleListDto> toggles, AuditDto audit)
            : base(audit)
        {
            Id = id;
            Name = name;
            _environments = environments?.ToList() ?? new List<EnvironmentListDto>();
            _toggles = toggles?.ToList() ?? new List<ToggleListDto>();
        }

        public Guid Id { get; }

        public string Name { get; }

        public IEnumerable<EnvironmentListDto> Environments => _environments;

        public IEnumerable<ToggleListDto> Toggles => _toggles;

        public static ProjectDetailsDto Create(EventAuditDto eventAudit, Guid id, string name)
        {
            return new ProjectDetailsDto(id, name, new List<EnvironmentListDto>(), new List<ToggleListDto>(), AuditDto.Create(eventAudit));
        }

        public static string StoreKey(Guid projectId)
        {
            return $"{nameof(ProjectDetailsDto)}-{projectId}";
        }

        public void AddEnvironment(EventAuditDto eventAudit, string environmentKey, string environmentName)
        {
            Audit.Update(eventAudit);

            var environment = new EnvironmentListDto(environmentKey, environmentName);
            _environments.Add(environment);
        }

        public void DeleteEnvironment(EventAuditDto eventAudit, string environmentKey)
        {
            Audit.Update(eventAudit);

            var environment = _environments.Single(e => e.Key == environmentKey);
            _environments.Remove(environment);
        }

        public void AddToggle(EventAuditDto eventAudit, string toggleKey, string toggleName)
        {
            Audit.Update(eventAudit);

            var toggleToAdd = new ToggleListDto(toggleKey, toggleName);
            _toggles.Add(toggleToAdd);
        }

        public void DeleteToggle(EventAuditDto eventAudit, string toggleKey)
        {
            Audit.Update(eventAudit);

            var toggleToRemove = _toggles.Single(toggle => toggle.Key == toggleKey);
            _toggles.Remove(toggleToRemove);
        }
    }
}
