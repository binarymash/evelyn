namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProjectDetailsDto : DtoRoot
    {
        private readonly IList<EnvironmentListDto> _environments;
        private readonly IList<ToggleListDto> _toggles;

        public ProjectDetailsDto(Guid id, string name, IEnumerable<EnvironmentListDto> environments, IEnumerable<ToggleListDto> toggles, int version, DateTimeOffset created, DateTimeOffset lastModified)
            : base(created, lastModified)
        {
            Id = id;
            Name = name;
            Version = version;
            _environments = environments.ToList();
            _toggles = toggles.ToList();
        }

        public Guid Id { get; }

        public string Name { get; }

        public int Version { get; private set; }

        public IEnumerable<EnvironmentListDto> Environments => _environments;

        public IEnumerable<ToggleListDto> Toggles => _toggles;
    }
}
