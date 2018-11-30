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

        public static EnvironmentStateDto Create(EventAuditDto eventAudit, IEnumerable<ToggleStateDto> toggleStates)
        {
            return new EnvironmentStateDto(toggleStates ?? new List<ToggleStateDto>(), AuditDto.Create(eventAudit));
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentStateDto)}-{projectId}-{environmentKey}";
        }

        public void AddToggleState(EventAuditDto eventAudit, string toggleKey, string toggleValue)
        {
            Audit.Update(eventAudit);

            var toggleState = new ToggleStateDto(toggleKey, toggleValue, eventAudit.NewVersion);
            _toggleStates.Add(toggleState);
        }

        public void ChangeToggleState(EventAuditDto eventAudit, string toggleKey, string value)
        {
            Audit.Update(eventAudit);

            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            toggleState.ChangeState(value, eventAudit.NewVersion);
        }

        public void DeleteToggleState(EventAuditDto eventAudit, string toggleKey)
        {
            Audit.Update(eventAudit);

            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            _toggleStates.Remove(toggleState);
        }
    }
}
