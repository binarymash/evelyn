namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;

    public abstract class DtoRoot
    {
        protected DtoRoot(long version, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
        {
            Audit = AuditDto.Create(created, createdBy, lastModified, lastModifiedBy, version);
        }

        public AuditDto Audit { get; protected set; }
    }
}
