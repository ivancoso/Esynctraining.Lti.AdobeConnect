namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(FileDTO))]
    public class DistractorFromStoredProcedureDTO
    {
        [DataMember]
        public virtual string distractor { get; set; }

        [DataMember]
        public virtual int distractorId { get; set; }

        [DataMember]
        public virtual int distractorOrder { get; set; }

        [DataMember]
        public virtual int? distractorType { get; set; }

        [DataMember]
        public virtual int? height { get; set; }

        [DataMember]
        public virtual Guid? imageId { get; set; }

        [DataMember]
        public virtual Guid? leftImageId { get; set; }

        [DataMember]
        public virtual Guid? rightImageId { get; set; }

        [DataMember]
        public virtual FileDTO imageVO { get; set; }

        [DataMember]
        public virtual FileDTO leftImageVO { get; set; }

        [DataMember]
        public virtual FileDTO rightImageVO { get; set; }

        [DataMember]
        public virtual bool? isCorrect { get; set; }

        [DataMember]
        public virtual int? questionId { get; set; }

        [DataMember]
        public virtual int? width { get; set; }

        [DataMember]
        public virtual int? x { get; set; }

        [DataMember]
        public virtual int? y { get; set; }

        [DataMember]
        public virtual int? rows { get; set; }

    }

}