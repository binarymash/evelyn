namespace Evelyn.Core.ReadModel.Projections.EventHandlerState
{
    using System;

    public class EventHandlerStateDto : DtoRoot
    {
        public EventHandlerStateDto(long version, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
            : base(version, created, createdBy, lastModified, lastModifiedBy)
        {
        }

        public static EventHandlerStateDto Null => new EventHandlerStateDto(-1, DateTimeOffset.UtcNow, Constants.SystemUser, DateTimeOffset.UtcNow, Constants.SystemUser);

        public static string StoreKey(Type eventStreamType)
        {
            return $"{nameof(eventStreamType)}-State";
        }

        public void Processed(long version, DateTimeOffset lastModified, string lastModifiedBy)
        {
            Audit.Update(lastModified, lastModifiedBy, version);
        }
    }
}
