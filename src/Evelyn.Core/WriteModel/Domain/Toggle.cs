namespace Evelyn.Core.WriteModel.Domain
{
    public class Toggle
    {
        public Toggle()
        {
        }

        public Toggle(string key, string name)
        {
            Name = name;
            Key = key;
        }

        public string Name { get; }

        public string Key { get; }
    }
}
