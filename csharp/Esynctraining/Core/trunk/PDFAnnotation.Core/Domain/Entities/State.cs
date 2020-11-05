namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The state
    /// </summary>
    public class State : Entity
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the state name.
        /// </summary>
        public virtual string StateName { get; set; }

        /// <summary>
        ///     Gets or sets the state code.
        /// </summary>
        public virtual string StateCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual Country Country { get; set; }

        #endregion
    }
}