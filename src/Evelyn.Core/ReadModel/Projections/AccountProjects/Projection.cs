namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System;
    using Newtonsoft.Json;

    public class Projection : Projections.Projection
    {
        [JsonConstructor]
        private Projection(ProjectionAudit audit, Model.Account account)
            : base(audit)
        {
            Account = account;
        }

        public Model.Account Account { get; private set; }

        public static string StoreKey(Guid accountId)
        {
            return $"{nameof(AccountProjects)}-{accountId}";
        }

        public static Projection Create(EventAudit eventAudit, Model.Account account)
        {
            return new Projection(
                ProjectionAudit.Create(eventAudit),
                account);
        }
    }
}
