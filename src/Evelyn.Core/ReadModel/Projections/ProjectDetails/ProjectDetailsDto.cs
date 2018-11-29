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

        public static ProjectDetailsDto Create(Guid id, string name, DateTimeOffset created, string createdBy, int version)
        {
            return new ProjectDetailsDto(id, name, new List<EnvironmentListDto>(), new List<ToggleListDto>(), AuditDto.Create(created, createdBy, version));
        }

        public static string StoreKey(Guid projectId)
        {
            return $"{nameof(ProjectDetailsDto)}-{projectId}";
        }

        public void AddEnvironment(string environmentKey, string environmentName, DateTimeOffset lastModified,  string lastModifiedBy, long lastModifiedVersion)
        {
            Audit.Update(lastModified, lastModifiedBy, lastModifiedVersion);

            var environment = new EnvironmentListDto(environmentKey, environmentName);
            _environments.Add(environment);
        }

        public void DeleteEnvironment(string environmentKey, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);

            var environment = _environments.Single(e => e.Key == environmentKey);
            _environments.Remove(environment);
        }

        public void AddToggle(string toggleKey, string toggleName, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);

            var toggleToAdd = new ToggleListDto(toggleKey, toggleName);
            _toggles.Add(toggleToAdd);
        }

        public void DeleteToggle(string toggleKey, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);

            var toggleToRemove = _toggles.Single(toggle => toggle.Key == toggleKey);
            _toggles.Remove(toggleToRemove);
        }
    }
}
