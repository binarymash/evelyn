namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ApplicationCreated : ApplicationEvent
    {
        public string Name { get; set; }

        public ApplicationCreated(Guid id, string name) : base(id)
        {
            Name = name;
        }
    }
}
