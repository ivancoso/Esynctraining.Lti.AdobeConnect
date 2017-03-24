namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Esynctraining.Core.Extensions;

    [DataContract]
    public class RecordingTransactionDTO
    {
        [IgnoreDataMember]
        [ScriptIgnore]
        public DateTime DateClosed { get; set; }

        [DataMember]
        public long? ClosedAt
        {
            get
            {
                if (DateClosed != DateTime.MinValue)
                    return (long)DateClosed.ConvertToUnixTimestamp();
                return null;
            }
            set
            {
            }
        }

        [IgnoreDataMember]
        [ScriptIgnore]
        public DateTime DateCreated { get; set; }

        [Required]
        [DataMember]
        public long CreatedAt
        {
            get
            {
                return (long)DateCreated.ConvertToUnixTimestamp();
            }
            set
            {
            }
        }

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
