namespace Evelyn.Management.Api.Rest.Write.Projects.Messages
{
    using System;

    public class CreateProject
    {
        public CreateProject(Guid projectId, string name)
        {
            ProjectId = projectId;
            Name = name;
        }

        public Guid ProjectId { get; set; }

        public string Name { get; set; }
    }
}
