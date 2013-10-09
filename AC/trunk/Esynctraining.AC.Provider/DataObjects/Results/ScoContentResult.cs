namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The SCO content result.
    /// </summary>
    public class ScoContentResult : GenericResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoContentResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public ScoContentResult(StatusInfo status)
            : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoContentResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public ScoContentResult(StatusInfo status, ScoContent value)
            : base(status)
        {
            this.ScoContent = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the SCO content.
        /// </summary>
        public ScoContent ScoContent { get; set; }

        #endregion
    }
}