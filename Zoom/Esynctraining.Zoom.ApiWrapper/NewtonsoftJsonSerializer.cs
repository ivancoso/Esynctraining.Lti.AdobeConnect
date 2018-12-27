using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp.Serializers;

namespace Esynctraining.Zoom.ApiWrapper
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public NewtonsoftJsonSerializer()
        {
            this.ContentType = "application/json";
            this._serializer = new Newtonsoft.Json.JsonSerializer()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                ContractResolver = (IContractResolver)new DefaultContractResolver()
                {
                    NamingStrategy = (NamingStrategy)new SnakeCaseNamingStrategy()
                }
            };
        }

        public NewtonsoftJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            this.ContentType = "application/json";
            this._serializer = serializer;
        }

        public string Serialize(object obj)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter)stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';
                    this._serializer.Serialize((JsonWriter)jsonTextWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }

        public string DateFormat { get; set; }

        public string RootElement { get; set; }

        public string Namespace { get; set; }

        public string ContentType { get; set; }
    }
}
