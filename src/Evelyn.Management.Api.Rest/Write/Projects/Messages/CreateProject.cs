namespace Evelyn.Management.Api.Rest.Write.Projects.Messages
{
    using System;

    public class CreateProject
    {
        public CreateProject(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
