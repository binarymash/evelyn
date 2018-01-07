namespace Evelyn.Core.ReadModel.ApplicationDetails
{
    using System;

    public class ToggleListDto
    {
        public ToggleListDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
