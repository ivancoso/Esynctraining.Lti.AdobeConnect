namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The meeting attendee collection result.
    /// </summary>
    public class QuizResponseCollectionResult : CollectionResult<QuizResponse>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResponseCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public QuizResponseCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResponseCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public QuizResponseCollectionResult(StatusInfo status, IEnumerable<QuizResponse> values)
            : base(status, values)
        {
        }

        #endregion
    }
}