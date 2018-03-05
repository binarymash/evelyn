namespace Evelyn.Core.ReadModel.Events
{
    using System;
    using CQRSlite.Events;

    public abstract class ProjectEvent : IEvent
    {
        protected ProjectEvent(string userId, Guid projectId)
        {
            UserId = userId;
            Id = projectId;
        }

        public string UserId { get; set; }

        public Guid Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
