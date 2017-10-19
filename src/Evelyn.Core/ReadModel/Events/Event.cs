namespace Evelyn.Core.ReadModel.Events
{
    using CQRSlite.Events;
    using System;

    public abstract class ApplicationEvent : IEvent
    {
        public Guid Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public ApplicationEvent(Guid id)
        {
            Id = id;
        }
    }
}
