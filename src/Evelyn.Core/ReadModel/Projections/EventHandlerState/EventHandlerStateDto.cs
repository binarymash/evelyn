namespace Evelyn.Core.ReadModel.Projections.EventHandlerState
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class EventHandlerStateDto : DtoRoot
    {
        [JsonConstructor]
        private EventHandlerStateDto(ProjectionAuditDto audit)
            : base(audit)
        {
        }

        public static EventHandlerStateDto Create()
        {
            var eventAudit = EventAuditDto.Create(DateTimeOffset.UtcNow, Constants.SystemUser, -1, -1);
            return new EventHandlerStateDto(ProjectionAuditDto.Create(eventAudit));
        }

        public static string StoreKey(Type eventStreamType)
        {
            return $"{nameof(eventStreamType)}-State";
        }

        public void Processed(EventAuditDto eventAudit)
        {
            Audit.Update(eventAudit);
        }
    }
}
