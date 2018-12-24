namespace Evelyn.Core.ReadModel.Projections.EnvironmentState.Model
{
    using Newtonsoft.Json;

    public class ToggleState
    {
        [JsonConstructor]
        private ToggleState(string key, string value, long version)
        {
            Key = key;
            Value = value;
            Version = version;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public long Version { get; private set; }

        public static ToggleState Create(string key, string value, long version)
        {
            return new ToggleState(key, value, version);
        }

        public void ChangeState(string value, long version)
        {
            Value = value;
            Version = version;
        }
    }
}
