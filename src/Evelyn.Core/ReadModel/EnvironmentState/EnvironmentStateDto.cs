namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentStateDto
    {
        private readonly List<ToggleStateDto> _toggleStates;

        public EnvironmentStateDto(int version, DateTimeOffset created, DateTimeOffset lastModified, IEnumerable<ToggleStateDto> toggleStates)
        {
            Version = version;
            Created = created;
            LastModified = lastModified;
            _toggleStates = toggleStates.ToList();
        }

        public DateTimeOffset Created { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public int Version { get; private set; }

        public IEnumerable<ToggleStateDto> ToggleStates => _toggleStates;
    }
}
