namespace Evelyn.Core.WriteModel.Account.Events
{
    using System;

    public class AccountRegistered : Event
    {
        public AccountRegistered(string userId, Guid accountId, DateTimeOffset occurredAt)
            : base(userId, accountId, occurredAt)
        {
            Version = -1;
        }
    }
}
