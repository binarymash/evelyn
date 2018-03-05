namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ToggleAdded : ProjectEvent
    {
        public ToggleAdded(string userId, Guid projectId, Guid toggleId, string name, string key)
            : base(userId, projectId)
        {
            ToggleId = toggleId;
            Name = name;
            Key = key;
        }

        public Guid ToggleId { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}
