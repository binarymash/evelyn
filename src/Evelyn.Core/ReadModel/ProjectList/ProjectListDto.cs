namespace Evelyn.Core.ReadModel.ProjectList
{
    using System;

    public class ProjectListDto
    {
        public ProjectListDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
