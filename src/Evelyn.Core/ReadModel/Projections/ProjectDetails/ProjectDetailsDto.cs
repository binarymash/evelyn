namespace Evelyn.Core.ReadModel.Projections.ProjectDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProjectDetailsDto : DtoRoot
    {
        private readonly IList<EnvironmentListDto> _environments;
        private readonly IList<ToggleListDto> _toggles;

        public ProjectDetailsDto(Guid id, string name, IEnumerable<EnvironmentListDto> environments, IEnumerable<ToggleListDto> toggles, int version, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
            : base(version, created, createdBy, lastModified, lastModifiedBy)
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

        public static string StoreKey(Guid projectId)
        {
            return $"{nameof(ProjectDetailsDto)}-{projectId}";
        }

        public void AddEnvironment(string environmentKey, string environmentName, DateTimeOffset lastModified, int lastModifiedVersion, string lastModifiedBy)
        {
            UpdateModificationAudit(lastModified, lastModifiedBy, lastModifiedVersion);

            var environment = new EnvironmentListDto(environmentKey, environmentName);
            _environments.Add(environment);
        }

        public void DeleteEnvironment(string environmentKey, DateTimeOffset lastModified, string lastModifiedBy, int lastModifiedVersion)
        {
            UpdateModificationAudit(lastModified, lastModifiedBy, lastModifiedVersion);

            var environment = _environments.Single(e => e.Key == environmentKey);
            _environments.Remove(environment);
        }

        public void AddToggle(string toggleKey, string toggleName, DateTimeOffset lastModified, string lastModifiedBy, int lastModifiedVersion)
        {
            UpdateModificationAudit(lastModified, lastModifiedBy, lastModifiedVersion);

            var toggleToAdd = new ToggleListDto(toggleKey, toggleName);
            _toggles.Add(toggleToAdd);
        }

        public void DeleteToggle(string toggleKey, DateTimeOffset lastModified, string lastModifiedBy, int lastModifiedVersion)
        {
            UpdateModificationAudit(lastModified, lastModifiedBy, lastModifiedVersion);

            var toggleToRemove = _toggles.Single(toggle => toggle.Key == toggleKey);
            _toggles.Remove(toggleToRemove);
        }
    }
}
