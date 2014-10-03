namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ConnectionInfoDTO
    {
        [DataMember]
        public string status { get; set; }
        
        [DataMember]
        public string info { get; set; }
    }
}
