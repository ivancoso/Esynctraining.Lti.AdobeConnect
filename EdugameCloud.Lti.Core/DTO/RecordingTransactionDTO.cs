namespace EdugameCloud.Lti.DTO
{
    using System;
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

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string RecordingName { get; set; }

        [DataMember]
        public string RecordingScoId { get; set; }

        [DataMember]
        public string UserName { get; set; }

    }

}
