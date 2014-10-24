namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The LMS user parameters.
    /// </summary>
    public class LmsUserParameters : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ac id.
        /// </summary>
        public virtual string AcId { get; set; }

        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        public virtual int Course { get; set; }

        /// <summary>
        /// Gets or sets the LMS user.
        /// </summary>
        public virtual LmsUser LmsUser { get; set; }

        /// <summary>
        /// Gets or sets the WS token.
        /// </summary>
        public virtual string Wstoken { get; set; }

        /// <summary>
        /// Gets or sets the company lms.
        /// </summary>
        public virtual CompanyLms CompanyLms { get; set; }

        #endregion
    }
}