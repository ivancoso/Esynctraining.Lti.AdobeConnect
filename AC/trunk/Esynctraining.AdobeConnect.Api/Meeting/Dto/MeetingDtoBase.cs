using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Meeting.Dto
{
    [DataContract]
    public class MeetingDtoBase
    {
        [DataMember]
        public string AcRoomUrl { get; set; }

        [DataMember]
        public string AccessLevel { get; set; }

        [DataMember]
        public bool CanJoin { get; set; }

        [Required]
        [DataMember]
        public string Duration { get; set; }

        //[DataMember] // was long
        //public string id { get; set; }

        [DataMember]
        public bool IsEditable { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }
        
        // Used in read actions
        [DataMember]
        public long StartTimeStamp { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [Required]
        [DataMember]
        public string Template { get; set; }
        
        [DataMember]
        public string AudioProfileId { get; set; }

        // Not in use in SSO
        [DataMember]
        public string AudioProfileName { get; set; }
        
    }

}
