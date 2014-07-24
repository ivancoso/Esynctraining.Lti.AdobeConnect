namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class MoodleUserInfoDTO
    {
        [DataMember]
        public virtual int userId { get; set; }
        [DataMember]
        public virtual string name { get; set; }
        [DataMember]
        public virtual string password { get; set; }
        [DataMember]
        public virtual string domain { get; set; }
        [DataMember]
        public virtual string provider { get; set; }
    }
}
