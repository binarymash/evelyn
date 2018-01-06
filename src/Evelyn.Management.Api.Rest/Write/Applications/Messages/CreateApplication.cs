namespace Evelyn.Management.Api.Rest.Write.Applications.Messages
{
    using System;

    public class CreateApplication
    {
        public CreateApplication(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
