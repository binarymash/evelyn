namespace Evelyn.Core.WriteModel.Evelyn.Events
{
    using System;

    public class SystemStarted : Event
    {
        public SystemStarted(string userId, Guid id, DateTimeOffset occurredAt)
            : base(userId, id, occurredAt)
        {
        }
    }
}
