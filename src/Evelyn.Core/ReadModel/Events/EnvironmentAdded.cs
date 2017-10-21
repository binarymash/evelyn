namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class EnvironmentAdded : ApplicationEvent
    {
        public EnvironmentAdded(Guid applicationId, Guid environmentId, string name, string key)
            : base(applicationId)
        {
            EnvironmentId = environmentId;
            Name = name;
            Key = key;
        }

        public Guid EnvironmentId { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}
