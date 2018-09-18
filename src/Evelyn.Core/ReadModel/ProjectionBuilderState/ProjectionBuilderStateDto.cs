namespace Evelyn.Core.ReadModel.ProjectionBuilderState
{
    using System;

    public class ProjectionBuilderStateDto : DtoRoot
    {
        public ProjectionBuilderStateDto(int version, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
            : base(version, created, createdBy, lastModified, lastModifiedBy)
        {
        }

        public void Processed(int version, DateTimeOffset lastModified, string lastModifiedBy)
        {
            this.UpdateModificationAudit(lastModified, lastModifiedBy, version);
        }
    }
}
