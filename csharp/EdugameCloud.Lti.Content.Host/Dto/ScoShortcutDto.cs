﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Content.Host.Dto
{
    [DataContract]
    public class ScoShortcutDto
    {
        [Required]
        [DataMember]
        public string ScoId { get; set; }

        [Required]
        [DataMember]
        public string Type { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

    }

}