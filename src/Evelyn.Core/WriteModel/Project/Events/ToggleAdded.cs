namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;

    public class ToggleAdded : Event
    {
        public ToggleAdded(string userId, Guid projectId, string key, string name, DateTimeOffset occurredAt)
            : base(userId, projectId, occurredAt)
        {
            Name = name;
            Key = key;
        }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}
