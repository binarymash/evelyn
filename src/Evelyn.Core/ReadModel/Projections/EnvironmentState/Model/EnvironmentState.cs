namespace Evelyn.Core.ReadModel.Projections.EnvironmentState.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class EnvironmentState
    {
        private readonly List<ToggleState> _toggleStates;

        [JsonConstructor]
        private EnvironmentState(IEnumerable<ToggleState> toggleStates)
        {
            _toggleStates = toggleStates.ToList();
        }

        public IEnumerable<ToggleState> ToggleStates => _toggleStates;

        public static EnvironmentState Create(EventAuditDto eventAudit, IEnumerable<ToggleState> toggleStates)
        {
            return new EnvironmentState(toggleStates ?? new List<ToggleState>());
        }

        public void AddToggleState(EventAuditDto eventAudit, string toggleKey, string toggleValue)
        {
            var toggleState = ToggleState.Create(toggleKey, toggleValue, eventAudit.EventVersion);
            _toggleStates.Add(toggleState);
        }

        public void ChangeToggleState(EventAuditDto eventAudit, string toggleKey, string value)
        {
            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            toggleState.ChangeState(value, eventAudit.EventVersion);
        }

        public void DeleteToggleState(EventAuditDto eventAudit, string toggleKey)
        {
            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            _toggleStates.Remove(toggleState);
        }
    }
}
