using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Meeting.Dto
{
    [DataContract]
    public class UserDtoBase
    {
        [DataMember(Name = "acId")]
        public string PrincipalId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Email { get; set; }

        // TODO: change to int32 via AcRole and use from LTI as well!!
        // 'ac_role' in LTI
        [DataMember]
        public string Role { get; set; }

    }
}