namespace Evelyn.Core.ReadModel.Events
{
    using System;
    using CQRSlite.Events;

    public abstract class ApplicationEvent : IEvent
    {
        protected ApplicationEvent(string userId, Guid applicationId)
        {
            UserId = userId;
            Id = applicationId;
        }

        public string UserId { get; set; }

        public Guid Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
