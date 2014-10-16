namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class RecordingDTO
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string begin_date { get; set; }

        [DataMember]
        public string end_date { get; set; }

        [DataMember]
        public int duration { get; set; }

        [DataMember]
        public string url { get; set; }
    }
}