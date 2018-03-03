namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ApplicationCreated : ApplicationEvent
    {
        public ApplicationCreated(string userId, string accountId, Guid applicationId, string name)
            : base(userId, applicationId)
        {
            AccountId = accountId;
            Name = name;
            Version = -1;
        }

        public string AccountId { get; set; }

        public string Name { get; set; }
    }
}
