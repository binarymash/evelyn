namespace Evelyn.Core.WriteModel
{
    using System;
    using CQRSlite.Events;

    public abstract class Event : IEvent
    {
        protected Event(string userId, Guid aggregateId)
        {
            UserId = userId;
            Id = aggregateId;
        }

        public string UserId { get; set; }

        public Guid Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
