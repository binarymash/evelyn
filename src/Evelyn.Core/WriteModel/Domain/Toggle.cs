using System;

namespace Evelyn.Core.WriteModel.Domain
{
    public class Toggle
    {
        public Toggle()
        {
        }

        public Toggle(Guid id, string name, string key)
        {
            Id = id;
            Name = name;
            Key = key;
        }

        public Guid Id { get; }

        public string Name { get; }

        public string Key { get; }
    }
}
