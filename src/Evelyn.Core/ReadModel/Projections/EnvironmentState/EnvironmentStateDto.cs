namespace Evelyn.Core.ReadModel.Projections.EnvironmentState
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

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentStateDto)}-{projectId}-{environmentKey}";
        }

        public void AddToggleState(string toggleKey, string toggleValue, int lastModifiedVersion, DateTimeOffset lastModified, string lastModifiedBy)
        {
            UpdateModificationAudit(lastModified, lastModifiedBy, lastModifiedVersion);

            var toggleState = new ToggleStateDto(toggleKey, toggleValue, Version);
            _toggleStates.Add(toggleState);
        }

        public void ChangeToggleState(string toggleKey, string value, int lastModifiedVersion, DateTimeOffset lastModified, string lastModifiedBy)
        {
            UpdateModificationAudit(lastModified, lastModifiedBy, lastModifiedVersion);

            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            toggleState.ChangeState(value, lastModifiedVersion);
        }

        public void DeleteToggleState(string toggleKey, int lastModifiedVersion, DateTimeOffset lastModified, string lastModifiedBy)
        {
            UpdateModificationAudit(lastModified, lastModifiedBy, lastModifiedVersion);

            var toggleState = _toggleStates.Find(ts => ts.Key == toggleKey);
            _toggleStates.Remove(toggleState);
        }
    }
}
