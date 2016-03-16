using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class SeminarSessionDto// : AdobeConnect.SeminarSessionDto
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string seminarRoomId { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string summary { get; set; }

        [DataMember]
        public string start_date { get; set; }

        [DataMember]
        public string start_time { get; set; }

        [DataMember]
        public long start_timestamp { get; set; }

        [DataMember]
        public string ac_room_url { get; set; }

        [DataMember]
        public string duration { get; set; }

        [DataMember]
        public bool is_editable { get; set; }

    }

}
