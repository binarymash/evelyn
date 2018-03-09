namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ToggleAdded : ProjectEvent
    {
        public ToggleAdded(string userId, Guid projectId, string key, string name)
            : base(userId, projectId)
        {
            Name = name;
            Key = key;
        }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}
