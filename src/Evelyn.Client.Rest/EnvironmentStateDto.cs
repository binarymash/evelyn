namespace Evelyn.Client.Rest
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.10.39.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class EnvironmentStateDto : System.ComponentModel.INotifyPropertyChanged
    {
        private System.Collections.ObjectModel.ObservableCollection<ToggleStateDto> _toggleStates;
        private System.DateTime? _created;
        private string _createdBy;
        private System.DateTime? _lastModified;
        private string _lastModifiedBy;
        private int? _version;

        [Newtonsoft.Json.JsonProperty("toggleStates", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.ObjectModel.ObservableCollection<ToggleStateDto> ToggleStates
        {
            get { return _toggleStates; }
            set
            {
                if (_toggleStates != value)
                {
                    _toggleStates = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("created", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTime? Created
        {
            get { return _created; }
            set
            {
                if (_created != value)
                {
                    _created = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("createdBy", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CreatedBy
        {
            get { return _createdBy; }
            set
            {
                if (_createdBy != value)
                {
                    _createdBy = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("lastModified", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTime? LastModified
        {
            get { return _lastModified; }
            set
            {
                if (_lastModified != value)
                {
                    _lastModified = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("lastModifiedBy", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string LastModifiedBy
        {
            get { return _lastModifiedBy; }
            set
            {
                if (_lastModifiedBy != value)
                {
                    _lastModifiedBy = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Version
        {
            get { return _version; }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static EnvironmentStateDto FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<EnvironmentStateDto>(data);
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

    }
}