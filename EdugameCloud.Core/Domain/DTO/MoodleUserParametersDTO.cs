namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    [DataContract]
    public class MoodleUserParametersDTO
    {
        public MoodleUserParametersDTO()
        {    
        }

        public MoodleUserParametersDTO(MoodleUserParameters param)
        {
            this.acId = param.AcId;
            this.course = param.Course;
            this.domain = param.Domain;
            this.provider = param.Provider;
            this.wstoken = param.Wstoken;
        }

        [DataMember]
        public virtual string acId { get; set; }
        [DataMember]
        public virtual string provider { get; set; }
        [DataMember]
        public virtual string wstoken { get; set; }
        [DataMember]
        public virtual int course { get; set; }
        [DataMember]
        public virtual string domain { get; set; }
        [DataMember]
        public virtual string errorMessage { get; set; }
        [DataMember]
        public virtual string errorDetails { get; set; }
    }
}
