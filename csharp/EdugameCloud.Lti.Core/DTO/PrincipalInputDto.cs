﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public class PrincipalInputDto
    {
        [DataMember]
        public string PrincipalId { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public bool SendEmail { get; set; }

        // TODO: IMPLEMENT!!
        [DataMember]
        public bool PromptPassword { get; set; }

        [Required]
        [DataMember]
        public int MeetingRole { get; set; }

    }

}
