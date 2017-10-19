using System;
using System.Collections.Generic;
using System.Linq;

namespace Evelyn.Core.WriteModel.Domain
{
    public class Environment
    {
        public IList<Guid> ToggleStates { get; }

        public Environment()
        {
            ToggleStates = new List<Guid>();
        }

        public Environment(Guid id, string name, string key) : this()
        {
            Id = id;
            Name = name;
            Key = key;
        }

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
