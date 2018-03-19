namespace Evelyn.Core.ReadModel.ProjectDetails
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
            _environments = environments.ToList();
            _toggles = toggles.ToList();
        }

        public Guid Id { get; }

        public string Name { get; }

        public IEnumerable<EnvironmentListDto> Environments => _environments;

        public IEnumerable<ToggleListDto> Toggles => _toggles;
    }
}
