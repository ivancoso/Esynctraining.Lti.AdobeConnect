namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The curriculum taker collection result.
    /// </summary>
    public class CurriculumTakerCollectionResult : CollectionResult<CurriculumTaker>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CurriculumTakerCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public CurriculumTakerCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurriculumTakerCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public CurriculumTakerCollectionResult(StatusInfo status, IEnumerable<CurriculumTaker> values)
            : base(status, values)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurriculumTakerCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        public CurriculumTakerCollectionResult(StatusInfo status, IEnumerable<CurriculumTaker> values, string scoId)
            : base(status, values)
        {
            this.ScoId = scoId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        public string ScoId { get; set; }

        #endregion
    }
}