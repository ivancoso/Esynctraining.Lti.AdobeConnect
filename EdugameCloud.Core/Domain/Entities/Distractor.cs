namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The distractor.
    /// </summary>
    public class Distractor : Entity
    {
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the distractor name.
        /// </summary>
        public virtual string DistractorName { get; set; }

        /// <summary>
        /// Gets or sets the distractor order.
        /// </summary>
        public virtual int DistractorOrder { get; set; }

        /// <summary>
        /// Gets or sets the distractor type.
        /// </summary>
        public virtual int? DistractorType { get; set; }

        /// <summary>
        /// Gets or sets the File.
        /// </summary>
        public virtual File Image { get; set; }
        public virtual File LeftImage { get; set; }
        public virtual File RightImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is correct.
        /// </summary>
        public virtual bool? IsCorrect { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        public virtual Question Question { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public virtual string Score { get; set; }

        /// <summary>
        /// Gets or sets the LMS answer.
        /// </summary>
        public virtual string LmsAnswer { get; set; }

        /// <summary>
        /// Gets or sets the LMS answer id.
        /// </summary>
        public virtual int? LmsAnswerId { get; set; }

        /// <summary>
        /// Gets or sets the LMS provider
        /// </summary>
        public virtual int? LmsProviderId { get; set; }

    }

}