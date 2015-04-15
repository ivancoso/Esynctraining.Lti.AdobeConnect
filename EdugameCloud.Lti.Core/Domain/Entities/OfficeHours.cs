namespace EdugameCloud.Lti.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The office hours.
    /// </summary>
    public class OfficeHours : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the sco id.
        /// </summary>
        public virtual string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the hours.
        /// </summary>
        public virtual string Hours { get; set; }

        /// <summary>
        /// Gets or sets the lms user.
        /// </summary>
        public virtual LmsUser LmsUser { get; set; }

        #endregion
    }
}
