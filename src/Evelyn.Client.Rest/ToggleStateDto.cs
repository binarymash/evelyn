namespace Evelyn.Client.Rest
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.10.39.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class ToggleStateDto : System.ComponentModel.INotifyPropertyChanged
    {
        private string _key;
        private string _value;

        [Newtonsoft.Json.JsonProperty("key", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Key
        {
            get { return _key; }
            set
            {
                if (_key != value)
                {
                    _key = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static ToggleStateDto FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ToggleStateDto>(data);
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

    }

    class ToggleStateDtoImpl : ToggleStateDto
    {
    }
}