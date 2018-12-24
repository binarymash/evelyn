namespace Evelyn.Core.ReadModel.Projections.ProjectDetails.Model
{
    using Newtonsoft.Json;

    public class Environment
    {
        [JsonConstructor]
        private Environment(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public string Key { get; }

        public string Name { get; }

        public static Environment Create(string key, string name)
        {
            return new Environment(key, name);
        }
    }
}
