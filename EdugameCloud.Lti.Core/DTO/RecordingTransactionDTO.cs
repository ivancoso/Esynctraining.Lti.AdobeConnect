namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Esynctraining.Core.Extensions;

    [DataContract]
    public class RecordingTransactionDTO
    {
        //[DataMember]
        [ScriptIgnore]
        public DateTime DateClosed { get; set; }

        //[DataMember]
        [ScriptIgnore]

        public DateTime DateCreated { get; set; }

        [DataMember(Name = "createdAt")]
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

        [DataMember(Name = "login")]
        public string Login { get; set; }

        [DataMember(Name = "recordingName")]
        public string RecordingName { get; set; }

        [DataMember(Name = "recordingScoId")]
        public string RecordingScoId { get; set; }

        [DataMember(Name = "userName")]
        public string UserName { get; set; }

    }

}
