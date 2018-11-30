namespace Evelyn.Core.ReadModel.Projections
{
    using Evelyn.Core.ReadModel.Projections.Shared;

    public abstract class DtoRoot
    {
        protected DtoRoot(AuditDto audit)
        {
            Audit = audit;
        }

        public AuditDto Audit { get; protected set; }
    }
}
