using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class PrincipalInputDto
    {
        [DataMember]
        public string principal_id { get; set; }

        [DataMember]
        public string login { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string firstName { get; set; }

        [DataMember]
        public string lastName { get; set; }

        [DataMember]
        public string password { get; set; }

        [DataMember]
        public bool sendEmail { get; set; }

        // TODO: IMPLEMENT!!
        [DataMember]
        public bool promptPassword { get; set; }

        [DataMember]
        public int meetingRole { get; set; }

    }

}
