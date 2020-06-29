namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class RecordingTransactionDTO
    {
        [DataMember(Name = "closedAt")]
        public DateTime? DateClosed { get; set; }

        [Required]
        [DataMember(Name = "createdAt")]
        public DateTime DateCreated { get; set; }

        [Required]
        [DataMember]
        public string Login { get; set; }

        [Required]
        [DataMember]
        public string RecordingName { get; set; }

        [Required]
        [DataMember]
        public string RecordingScoId { get; set; }

        [Required]
        [DataMember]
        public string UserName { get; set; }

    }

}
