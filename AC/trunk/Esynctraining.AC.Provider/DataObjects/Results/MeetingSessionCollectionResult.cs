namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The meeting attendee collection result.
    /// </summary>
    public class MeetingSessionCollectionResult : GenericCollectionResultBase<MeetingSession>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingSessionCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public MeetingSessionCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingSessionCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public MeetingSessionCollectionResult(StatusInfo status, IEnumerable<MeetingSession> values)
            : base(status, values)
        {
        }

        #endregion
    }
}