using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class ConnectionTestDTO
    {
        [DataMember]
        public string domain { get; set; }

        [DataMember]
        public bool enableProxyToolMode { get; set; }

        [DataMember]
        public string login { get; set; }

        [DataMember]
        public string password { get; set; }

        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string consumerKey { get; set; }

        [DataMember]
        public string consumerSecret { get; set; }

        [DataMember]
        public string token { get; set; }

        [DataMember]
        public string tokenSecret { get; set; }
    }

}
