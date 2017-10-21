namespace Evelyn.Core.ReadModel.Events
{
    using System;
    using CQRSlite.Events;

    public abstract class ApplicationEvent : IEvent
    {
        public ApplicationEvent(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
