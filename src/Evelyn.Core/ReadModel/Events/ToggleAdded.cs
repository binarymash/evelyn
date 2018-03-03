namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ToggleAdded : ApplicationEvent
    {
        public ToggleAdded(string userId, Guid applicationId, Guid toggleId, string name, string key)
            : base(userId, applicationId)
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
