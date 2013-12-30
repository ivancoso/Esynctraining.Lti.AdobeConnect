namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     The distractor from stored procedure dto.
    /// </summary>
    [DataContract]
    [KnownType(typeof(FileDTO))]
    public class DistractorFromStoredProcedureDTO
    {
        #region Public Properties

        [DataMember]
        public virtual int? questionId { get; set; }

        [DataMember]
        public virtual int distractorId { get; set; }

        [DataMember]
        public virtual int? distractorType { get; set; }
       
        [DataMember]
        public virtual string distractor { get; set; }

        [DataMember]
        public virtual bool? isCorrect { get; set; }

        [DataMember]
        public virtual int? imageId { get; set; }

        [DataMember]
        public virtual FileDTO imageVO { get; set; }

        [DataMember]
        public virtual int distractorOrder { get; set; }

        [DataMember]
        public virtual int? height { get; set; }

        [DataMember]
        public virtual int? width { get; set; }

        [DataMember]
        public virtual int? x { get; set; }

        [DataMember]
        public virtual int? y { get; set; }

        #endregion
    }
}