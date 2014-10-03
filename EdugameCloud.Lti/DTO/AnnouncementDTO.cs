namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AnnouncementDTO
    {
        [DataMember]
        public int id { get; set; }
        
        [DataMember]
        public string title { get; set; }

        [DataMember]
        public string message { get; set; }
    }
}