namespace Evelyn.Core.WriteModel.Evelyn.Events
{
    using System;

    public class SystemCreated : Event
    {
        public SystemCreated(string userId, Guid id, DateTimeOffset occurredAt)
            : base(userId, id, occurredAt)
        {
            Version = -1;
        }
    }
}
