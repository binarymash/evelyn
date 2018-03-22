namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentStateDto : DtoRoot
    {
        private readonly List<ToggleStateDto> _toggleStates;

        public EnvironmentStateDto(int version, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy, IEnumerable<ToggleStateDto> toggleStates)
            : base(version, created, createdBy, lastModified, lastModifiedBy)
        {
            _toggleStates = toggleStates.ToList();
        }

        public IEnumerable<ToggleStateDto> ToggleStates => _toggleStates;
    }
}
