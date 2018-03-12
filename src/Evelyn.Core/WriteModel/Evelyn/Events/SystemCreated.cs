namespace Evelyn.Core.WriteModel.Evelyn.Events
{
    using System;

    public class SystemCreated : Event
    {
        public SystemCreated(string userId, Guid id)
            : base(userId, id)
        {
            Version = -1;
        }
    }
}
