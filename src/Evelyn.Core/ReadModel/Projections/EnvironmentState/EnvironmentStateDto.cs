namespace Evelyn.Core.ReadModel.Projections.EnvironmentState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class EnvironmentStateDto : DtoRoot
    {
        private readonly List<ToggleStateDto> _toggleStates;

        [JsonConstructor]
        private EnvironmentStateDto(IEnumerable<ToggleStateDto> toggleStates, AuditDto audit)
            : base(audit)
        {
            _toggleStates = toggleStates.ToList();
        }

        public IEnumerable<ToggleStateDto> ToggleStates => _toggleStates;

        public static EnvironmentStateDto Create(IEnumerable<ToggleStateDto> toggleStates, DateTimeOffset created, string createdBy, long version)
        {
            return new EnvironmentStateDto(toggleStates ?? new List<ToggleStateDto>(), AuditDto.Create(created, createdBy, version));
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentStateDto)}-{projectId}-{environmentKey}";
        }

        public void AddToggleState(string toggleKey, string toggleValue, DateTimeOffset lastModified, string lastModifiedBy, long lastModifiedVersion)
        {
            Audit.Update(lastModified, lastModifiedBy, lastModifiedVersion);

            var toggleState = new ToggleStateDto(toggleKey, toggleValue, lastModifiedVersion);
            _toggleStates.Add(toggleState);
        }

        public void ChangeToggleState(string toggleKey, string value, DateTimeOffset lastModified, string lastModifiedBy, long lastModifiedVersion)
        {
            Audit.Update(lastModified, lastModifiedBy, lastModifiedVersion);

            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            toggleState.ChangeState(value, lastModifiedVersion);
        }

        public void DeleteToggleState(string toggleKey, DateTimeOffset lastModified, string lastModifiedBy, long lastModifiedVersion)
        {
            Audit.Update(lastModified, lastModifiedBy, lastModifiedVersion);

            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            _toggleStates.Remove(toggleState);
        }
    }
}
