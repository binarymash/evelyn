namespace Evelyn.Core.ReadModel.Projections.ClientEnvironmentState.Model
{
    using System.Collections.Generic;
    using System.Linq;
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

        public static EnvironmentState Create(EventAudit eventAudit, IEnumerable<ToggleState> toggleStates)
        {
            return new EnvironmentState(toggleStates ?? new List<ToggleState>());
        }

        public void AddToggleState(EventAudit eventAudit, string toggleKey, string toggleValue)
        {
            var toggleState = ToggleState.Create(toggleKey, toggleValue);
            _toggleStates.Add(toggleState);
        }

        public void ChangeToggleState(EventAudit eventAudit, string toggleKey, string value)
        {
            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            toggleState.ChangeState(value, eventAudit.EventVersion);
        }

        public void DeleteToggleState(EventAudit eventAudit, string toggleKey)
        {
            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            _toggleStates.Remove(toggleState);
        }
    }
}
