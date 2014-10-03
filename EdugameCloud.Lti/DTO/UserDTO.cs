namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class UserDTO
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string ac_id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string login_id { get; set; }

        [DataMember]
        public string canvas_role { get; set; }

        [DataMember]
        public string ac_role { get; set; }

        [DataMember]
        public string email { get; set; }
    }
}