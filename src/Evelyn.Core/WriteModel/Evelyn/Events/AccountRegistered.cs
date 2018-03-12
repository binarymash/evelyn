namespace Evelyn.Core.WriteModel.Evelyn.Events
{
    using System;

    public class AccountRegistered : Event
    {
        public AccountRegistered(string userId, Guid evelynId, Guid accountId)
            : base(userId, evelynId)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; set; }
    }
}
