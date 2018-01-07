namespace Evelyn.Core.WriteModel.Domain
{
    using System;
    using System.Collections.Generic;

    public class Environment
    {
        public Environment()
        {
            ToggleStates = new List<Guid>();
        }

        public Environment(Guid id, string name)
            : this()
        {
            Id = id;
            Name = name;
        }

        public IList<Guid> ToggleStates { get; }

        public Guid Id { get; }

        public string Name { get; }

        public void Toggle(Guid toggleId)
        {
            if (ToggleStates.Contains(toggleId))
            {
                ToggleStates.Remove(toggleId);
            }
            else
            {
                ToggleStates.Add(toggleId);
            }
        }
    }
}
