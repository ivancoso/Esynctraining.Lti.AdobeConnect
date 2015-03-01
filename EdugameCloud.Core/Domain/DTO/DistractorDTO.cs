namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The distractor DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(FileDTO))]
    public class DistractorDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DistractorDTO"/> class.
        /// </summary>
        public DistractorDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistractorDTO"/> class.
        /// </summary>
        /// <param name="distractor">
        /// The distractor.
        /// </param>
        public DistractorDTO(Distractor distractor)
        {
            this.distractorId = distractor.Id;
            this.questionId = distractor.Question.Return(x => x.Id, (int?)null);
            this.imageId = distractor.Image.Return(x => x.Id, (Guid?)null);
            this.distractor = distractor.DistractorName;
            this.distractorOrder = distractor.DistractorOrder;
            this.score = distractor.Score;
            this.isCorrect = distractor.IsCorrect;
            this.createdBy = distractor.CreatedBy.Return(x => x.Id, (int?)null);
            this.modifiedBy = distractor.ModifiedBy.Return(x => x.Id, (int?)null);
            this.dateCreated = distractor.DateCreated.ConvertToUnixTimestamp();
            this.dateModified = distractor.DateModified.ConvertToUnixTimestamp();
            this.isActive = distractor.IsActive;
            this.imageVO = distractor.Image.Return(x => new FileDTO(x), null);
            this.distractorType = distractor.DistractorType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? createdBy { get; set; }

        /// <summary>
        ///     Gets or sets the distractor type.
        /// </summary>
        [DataMember]
        public int? distractorType { get; set; }

        /// <summary>
        /// Gets or sets the image vo.
        /// </summary>
        [DataMember]
        public FileDTO imageVO { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the distractor.
        /// </summary>
        [DataMember]
        public string distractor { get; set; }

        /// <summary>
        /// Gets or sets the distractor id.
        /// </summary>
        [DataMember]
        public int distractorId { get; set; }

        /// <summary>
        /// Gets or sets the distractor order.
        /// </summary>
        [DataMember]
        public int distractorOrder { get; set; }

        /// <summary>
        /// Gets or sets the image id.
        /// </summary>
        [DataMember]
        public Guid? imageId { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets the is correct.
        /// </summary>
        [DataMember]
        public bool? isCorrect { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int? modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        [DataMember]
        public int? questionId { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        [DataMember]
        public string score { get; set; }

        #endregion
    }
}