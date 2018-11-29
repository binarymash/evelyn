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

        public static EnvironmentStateDto Create(IEnumerable<ToggleStateDto> toggleStates, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            return new EnvironmentStateDto(toggleStates ?? new List<ToggleStateDto>(), AuditDto.Create(occurredAt, initiatedBy, newVersion));
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentStateDto)}-{projectId}-{environmentKey}";
        }

        public void AddToggleState(string toggleKey, string toggleValue, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);

            var toggleState = new ToggleStateDto(toggleKey, toggleValue, newVersion);
            _toggleStates.Add(toggleState);
        }

        public void ChangeToggleState(string toggleKey, string value, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);

            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            toggleState.ChangeState(value, newVersion);
        }

        public void DeleteToggleState(string toggleKey, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);

            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            _toggleStates.Remove(toggleState);
        }
    }
}
