namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The distractor from stored procedure DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(FileDTO))]
    public class DistractorFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the distractor.
        /// </summary>
        [DataMember]
        public virtual string distractor { get; set; }

        /// <summary>
        /// Gets or sets the distractor id.
        /// </summary>
        [DataMember]
        public virtual int distractorId { get; set; }

        /// <summary>
        /// Gets or sets the distractor order.
        /// </summary>
        [DataMember]
        public virtual int distractorOrder { get; set; }

        /// <summary>
        /// Gets or sets the distractor type.
        /// </summary>
        [DataMember]
        public virtual int? distractorType { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [DataMember]
        public virtual int? height { get; set; }

        /// <summary>
        /// Gets or sets the image id.
        /// </summary>
        [DataMember]
        public virtual Guid? imageId { get; set; }

        /// <summary>
        /// Gets or sets the image vo.
        /// </summary>
        [DataMember]
        public virtual FileDTO imageVO { get; set; }

        /// <summary>
        /// Gets or sets the is correct.
        /// </summary>
        [DataMember]
        public virtual bool? isCorrect { get; set; }

        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        [DataMember]
        public virtual int? questionId { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [DataMember]
        public virtual int? width { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        [DataMember]
        public virtual int? x { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        [DataMember]
        public virtual int? y { get; set; }

        #endregion
    }
}