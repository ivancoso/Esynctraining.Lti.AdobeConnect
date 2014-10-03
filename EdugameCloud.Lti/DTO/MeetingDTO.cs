namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class MeetingDTO
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string template { get; set; }

        [DataMember]
        public string summary { get; set; }

        [DataMember]
        public string ac_room_url { get; set; }

        [DataMember]
        public string start_date { get; set; }

        [DataMember]
        public string start_time { get; set; }

        [DataMember]
        public string duration { get; set; }

        [DataMember]
        public string connect_server { get; set; }

        [DataMember]
        public string access_level { get; set; }

        [DataMember]
        public bool is_editable { get; set; }

        [DataMember]
        public bool can_join { get; set; }
    }
}