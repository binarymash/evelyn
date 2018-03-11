namespace Evelyn.Core.WriteModel.Account.Events
{
    using System;

    public class AccountRegistered : Event
    {
        public AccountRegistered(string userId, Guid accountId)
            : base(userId, accountId)
        {
            AccountId = accountId;
            Version = -1;
        }

        public Guid AccountId { get; set; }
    }
}
