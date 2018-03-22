namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;

    public class ProjectListDto
    {
        public ProjectListDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }
    }
}
