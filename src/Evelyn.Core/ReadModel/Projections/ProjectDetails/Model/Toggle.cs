namespace Evelyn.Core.ReadModel.Projections.ProjectDetails.Model
{
    using Newtonsoft.Json;

    public class Toggle
    {
        [JsonConstructor]
        private Toggle(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public string Key { get; }

        public string Name { get; }

        public static Toggle Create(string key, string name)
        {
            return new Toggle(key, name);
        }
    }
}
