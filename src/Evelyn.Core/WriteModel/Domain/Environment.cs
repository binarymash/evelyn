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

        public Environment(Guid id, string name, string key)
            : this()
        {
            Id = id;
            Name = name;
            Key = key;
        }

        public IList<Guid> ToggleStates { get; }

        public Guid Id { get; }

        public string Name { get; }

        public string Key { get; }

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
