namespace Evelyn.Core.ReadModel.Projections.ToggleState.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class ToggleState
    {
        private readonly List<EnvironmentState> _environmentStates;

        [JsonConstructor]
        private ToggleState(IEnumerable<EnvironmentState> environmentStates)
        {
            _environmentStates = environmentStates.ToList();
        }

        public IEnumerable<EnvironmentState> EnvironmentStates => _environmentStates;

        public static ToggleState Create(EventAudit eventAudit)
        {
            return new ToggleState(new List<EnvironmentState>());
        }

        public void AddEnvironmentState(EventAudit eventAudit, string environmentKey, string environmentValue)
        {
            var environmentState = EnvironmentState.Create(environmentKey, environmentValue, eventAudit.EventVersion);
            _environmentStates.Add(environmentState);
        }

        public void ChangeEnvironmentState(EventAudit eventAudit, string environmentKey, string value)
        {
            var environmentState = _environmentStates.Find(ts => ts.Key == environmentKey);
            environmentState.ChangeState(value, eventAudit.EventVersion);
        }

        public void DeleteEnvironmentState(EventAudit eventAudit, string environmentKey)
        {
            var environmentState = _environmentStates.Find(ts => ts.Key == environmentKey);
            _environmentStates.Remove(environmentState);
        }
    }
}
