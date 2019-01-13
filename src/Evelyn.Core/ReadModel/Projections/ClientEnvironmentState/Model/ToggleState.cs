namespace Evelyn.Core.ReadModel.Projections.ClientEnvironmentState.Model
{
    using Newtonsoft.Json;

    public class ToggleState
    {
        [JsonConstructor]
        private ToggleState(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public static ToggleState Create(string key, string value)
        {
            return new ToggleState(key, value);
        }

        public void ChangeState(string value, long version)
        {
            Value = value;
        }
    }
}
