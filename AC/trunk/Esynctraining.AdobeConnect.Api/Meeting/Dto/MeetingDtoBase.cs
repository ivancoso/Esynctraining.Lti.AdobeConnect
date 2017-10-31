using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect.Api.Meeting.Dto
{
    [DataContract]
    public class MeetingDtoBase
    {
        /// <summary>
        /// AC metting's url-path.
        /// </summary>
        //[Required] //todo: different dto for create and update
        [DataMember]
        public string AcRoomUrl { get; set; }

        [DataMember]
        [Obsolete("TRICK: to hide from Swagger only.")]
        public string AccessLevel { get; set; }

        [DataMember]
        [Obsolete("TRICK: to hide from Swagger only.")]
        public bool CanJoin { get; set; }

        /// <summary>
        /// Duration in format "h\:mm"
        /// </summary>
        //[Required]
        [DataMember]
        public string Duration { get; set; }

        //[DataMember] // was long
        //public string id { get; set; }

        [DataMember]
        [Obsolete("TRICK: to hide from Swagger only.")]
        public bool IsEditable { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

        // TRICK: Used in read-only actions. be careful with validation!!
        /// <summary>
        /// Unix timestamp value of meeting start date\time.
        /// </summary>
        //[Required]
        [DataMember]
        public long StartTimeStamp { get; set; }

        [DataMember]
        public string Summary { get; set; }

        /// <summary>
        /// sco-id of template used to create a meeting.
        /// </summary>
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
