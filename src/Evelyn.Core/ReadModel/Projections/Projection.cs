namespace Evelyn.Core.ReadModel.Projections
{
    public abstract class Projection
    {
        protected Projection(ProjectionAudit audit)
        {
            Audit = audit;
        }

        public ProjectionAudit Audit { get; protected set; }
    }
}
