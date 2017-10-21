namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ToggleAdded : ApplicationEvent
    {
        public ToggleAdded(Guid id, Guid toggleId, string name, string key)
            : base(id)
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
