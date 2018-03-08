namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class EnvironmentAdded : ProjectEvent
    {
        public EnvironmentAdded(string userId, Guid projectId, string key)
            : base(userId, projectId)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}
