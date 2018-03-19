namespace Evelyn.Core.ReadModel.EnvironmentState
{
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentStateDto
    {
        private readonly List<ToggleStateDto> _toggleStates = new List<ToggleStateDto>();

        public EnvironmentStateDto(int version, IEnumerable<ToggleStateDto> toggleStates)
        {
            Version = version;
            _toggleStates = toggleStates.ToList();
        }

        public int Version { get; private set; }

        public IEnumerable<ToggleStateDto> ToggleStates => _toggleStates;
    }
}
