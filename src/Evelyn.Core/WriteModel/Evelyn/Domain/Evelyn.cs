namespace Evelyn.Core.WriteModel.Evelyn.Domain
{
    using System;
    using System.Collections.Generic;
    using Account.Domain;
    using Events;
    using Newtonsoft.Json;

    public class Evelyn : EvelynAggregateRoot
    {
        [JsonProperty("Accounts")]
        private readonly IList<Guid> _accounts;

        public Evelyn()
        {
            Version = -1;
            _accounts = new List<Guid>();
        }

        public Evelyn(string userId, Guid id)
            : this()
        {
            ApplyChange(new SystemCreated(userId, id, DateTimeOffset.UtcNow));
        }

        [JsonIgnore]
        public IEnumerable<Guid> Accounts => _accounts;

        public Account RegisterAccount(string userId, Guid accountId)
        {
            if (_accounts.Contains(accountId))
            {
                throw new InvalidOperationException($"There is already an account with the ID {accountId}");
            }

            ApplyChange(new AccountRegistered(userId, this.Id, accountId, DateTimeOffset.UtcNow));
            return new Account(userId, accountId);
        }

        public void StartSystem(string userId)
        {
            ApplyChange(new SystemStarted(userId, this.Id, DateTimeOffset.UtcNow));
        }

        private void Apply(SystemCreated @event)
        {
            Id = @event.Id;
            Created = @event.OccurredAt;
            CreatedBy = @event.UserId;
            LastModified = @event.OccurredAt;
            LastModifiedBy = @event.UserId;
        }

        private void Apply(SystemStarted @event)
        {
            LastModified = @event.OccurredAt;
            LastModifiedBy = @event.UserId;
        }

        private void Apply(AccountRegistered @event)
        {
            _accounts.Add(@event.AccountId);
            LastModified = @event.OccurredAt;
            LastModifiedBy = @event.UserId;
        }
    }
}
