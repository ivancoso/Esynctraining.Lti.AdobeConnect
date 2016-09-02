namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The meeting item collection result.
    /// </summary>
    public class MeetingItemCollectionResult : CollectionResult<MeetingItem>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingItemCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public MeetingItemCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingItemCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public MeetingItemCollectionResult(StatusInfo status, IEnumerable<MeetingItem> values)
            : base(status, values)
        {
        }

        #endregion
    }
}