using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Models
{
    // TODO: move
    [DataContract]
    public class MeetingReuseDTO
    {
        [Required]
        [DataMember]
        public string ScoId { get; set; }

        [Required]
        [DataMember]
        public bool MergeUsers { get; set; }

        [Required]
        [DataMember]
        public int Type { get; set; }


        public LmsMeetingType GetMeetingType()
        {
            if (Type <= 0)
                throw new InvalidOperationException($"Invalid meeting type '{Type}'");
            return (LmsMeetingType)Type;
        }

    }
}
