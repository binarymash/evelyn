namespace Evelyn.Core.WriteModel.Evelyn.Events
{
    using System;

    public class AccountRegistered : Event
    {
        public AccountRegistered(string userId, Guid evelynId, Guid accountId, DateTimeOffset occurredAt)
            : base(userId, evelynId, occurredAt)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; set; }
    }
}
