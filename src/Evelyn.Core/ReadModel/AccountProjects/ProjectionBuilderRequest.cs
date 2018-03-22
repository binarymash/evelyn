namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;

    public class ProjectionBuilderRequest
    {
        public ProjectionBuilderRequest(Guid accountId)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; }
    }
}
