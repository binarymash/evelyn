namespace Evelyn.Core.ReadModel.Projections
{
    using Evelyn.Core.ReadModel.Projections.Shared;

    public abstract class Projection
    {
        protected Projection(ProjectionAuditDto audit)
        {
            Audit = audit;
        }

        public ProjectionAuditDto Audit { get; protected set; }
    }
}
