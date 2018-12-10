namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAuditDto audit, Model.Account account)
            : base(audit)
        {
            Account = account;
        }

        public Model.Account Account { get; private set; }

        public static string StoreKey(Guid accountId)
        {
            return $"{nameof(AccountProjects)}-{accountId}";
        }

        public static Projection Create(EventAuditDto eventAudit, Model.Account account)
        {
            return new Projection(
                ProjectionAuditDto.Create(eventAudit),
                account);
        }
    }
}
