namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ToggleAdded : ApplicationEvent
    {
        public Guid ToggleId { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public ToggleAdded(Guid id, Guid toggleId, string name, string key) 
            : base(id)
        {
            ToggleId = toggleId;
            Name = name;
            Key = key;
        }
    }
}
