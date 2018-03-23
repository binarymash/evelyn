namespace Evelyn.Client.Rest
{
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "11.16.0.0 (NJsonSchema v9.10.39.0 (Newtonsoft.Json v9.0.0.0))")]
    public partial class Client
    {
        private string _baseUrl = "";
        private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;

        public Client(string baseUrl)
        {
            BaseUrl = baseUrl;
            _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(() =>
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                UpdateJsonSerializerSettings(settings);
                return settings;
            });
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        protected Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings { get { return _settings.Value; } }

        partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response);

        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<EnvironmentStateDto> ApiStatesByProjectIdByEnvironmentNameGetAsync(System.Guid projectId, string environmentName)
        {
            return ApiStatesByProjectIdByEnvironmentNameGetAsync(projectId, environmentName, System.Threading.CancellationToken.None);
        }

        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<EnvironmentStateDto> ApiStatesByProjectIdByEnvironmentNameGetAsync(System.Guid projectId, string environmentName, System.Threading.CancellationToken cancellationToken)
        {
            if (projectId == null)
                throw new System.ArgumentNullException("projectId");

            if (environmentName == null)
                throw new System.ArgumentNullException("environmentName");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/states/{projectId}/{environmentName}");
            urlBuilder_.Replace("{projectId}", System.Uri.EscapeDataString(ConvertToString(projectId, System.Globalization.CultureInfo.InvariantCulture)));
            urlBuilder_.Replace("{environmentName}", System.Uri.EscapeDataString(ConvertToString(environmentName, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = new System.Net.Http.HttpClient();
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "500")
                        {
                            var responseData_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var result_ = default(System.Collections.Generic.Dictionary<string, string>);
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, string>>(responseData_, _settings.Value);
                            }
                            catch (System.Exception exception_)
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                            throw new SwaggerException<System.Collections.Generic.Dictionary<string, string>>("Server Error", (int)response_.StatusCode, responseData_, headers_, result_, null);
                        }
                        else
                        if (status_ == "200")
                        {
                            var responseData_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var result_ = default(EnvironmentStateDto);
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<EnvironmentStateDto>(responseData_, _settings.Value);
                                return result_;
                            }
                            catch (System.Exception exception_)
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                        }
                        else
                        if (status_ == "404")
                        {
                            var responseData_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var result_ = default(System.Collections.Generic.Dictionary<string, string>);
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, string>>(responseData_, _settings.Value);
                            }
                            catch (System.Exception exception_)
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                            throw new SwaggerException<System.Collections.Generic.Dictionary<string, string>>("Not Found", (int)response_.StatusCode, responseData_, headers_, result_, null);
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }

                        return default(EnvironmentStateDto);
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (client_ != null)
                    client_.Dispose();
            }
        }

        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is System.Enum)
            {
                string name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute))
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value;
                        }
                    }
                }
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[])value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array)value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }

            return System.Convert.ToString(value, cultureInfo);
        }
    }
}