﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class MeetingSessionDTO
    {
        [Required]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string EventId { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

        [Required]
        [DataMember]
        public DateTime StartDate { get; set; }

        [Required]
        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public string Summary { get; set; }

    }

}