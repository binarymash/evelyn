namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentStateDto : DtoRoot
    {
        private readonly List<ToggleStateDto> _toggleStates;

        public EnvironmentStateDto(int version, DateTimeOffset created, DateTimeOffset lastModified, IEnumerable<ToggleStateDto> toggleStates)
            : base(created, lastModified)
        {
            Version = version;
            _toggleStates = toggleStates.ToList();
        }

        public int Version { get; private set; }

        public IEnumerable<ToggleStateDto> ToggleStates => _toggleStates;
    }
}
