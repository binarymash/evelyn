﻿namespace Evelyn.Core.ReadModel
{
    using CQRSlite.Events;

    public class EventEnvelope
    {
        public EventEnvelope(long streamVersion, IEvent @event)
        {
            StreamVersion = streamVersion;
            Event = @event;
        }

        public long StreamVersion { get; }

        public IEvent Event { get; }
    }
}
