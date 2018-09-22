namespace Evelyn.Core.ReadModel.EventStreamHandlerState
{
    using System;

    public class EventStreamHandlerStateDto : DtoRoot
    {
        public EventStreamHandlerStateDto(long version, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
            : base(version, created, createdBy, lastModified, lastModifiedBy)
        {
        }

        public static EventStreamHandlerStateDto Null => new EventStreamHandlerStateDto(-1, DateTimeOffset.UtcNow, Constants.SystemUser, DateTimeOffset.UtcNow, Constants.SystemUser);

        public void Processed(long version, DateTimeOffset lastModified, string lastModifiedBy)
        {
            this.UpdateModificationAudit(lastModified, lastModifiedBy, version);
        }
    }
}
