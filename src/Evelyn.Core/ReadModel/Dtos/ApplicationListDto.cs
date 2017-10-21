namespace Evelyn.Core.ReadModel.Dtos
{
    using System;

    public class ApplicationListDto
    {
        public ApplicationListDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
