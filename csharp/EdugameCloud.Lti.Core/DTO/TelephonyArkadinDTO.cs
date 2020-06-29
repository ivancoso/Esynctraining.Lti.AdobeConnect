using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class TelephonyArkadinDTO
    {
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

    }

}
