namespace Evelyn.Core.ReadModel.Projections.EventHandlerState
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class EventHandlerStateDto : DtoRoot
    {
        [JsonConstructor]
        private EventHandlerStateDto(AuditDto audit)
            : base(audit)
        {
        }

        public static EventHandlerStateDto Create()
        {
            return new EventHandlerStateDto(AuditDto.Create(DateTimeOffset.UtcNow, Constants.SystemUser, -1));
        }

        public static string StoreKey(Type eventStreamType)
        {
            return $"{nameof(eventStreamType)}-State";
        }

        public void Processed(DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            Audit.Update(occurredAt, initiatedBy, newVersion);
        }
    }
}
