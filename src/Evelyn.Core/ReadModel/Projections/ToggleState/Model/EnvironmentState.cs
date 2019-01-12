namespace Evelyn.Core.ReadModel.Projections.ToggleState.Model
{
    using Newtonsoft.Json;

    public class EnvironmentState
    {
        [JsonConstructor]
        private EnvironmentState(string key, string value, long version)
        {
            Key = key;
            Value = value;
            Version = version;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public long Version { get; private set; }

        public static EnvironmentState Create(string key, string value, long version)
        {
            return new EnvironmentState(key, value, version);
        }

        public void ChangeState(string value, long version)
        {
            Value = value;
            Version = version;
        }
    }
}
