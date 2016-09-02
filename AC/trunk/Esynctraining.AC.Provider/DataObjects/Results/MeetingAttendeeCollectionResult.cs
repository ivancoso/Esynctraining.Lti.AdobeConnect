namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    /// <summary>
    /// The meeting attendee collection result.
    /// </summary>
    public class MeetingAttendeeCollectionResult : CollectionResult<MeetingAttendee>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingAttendeeCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public MeetingAttendeeCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingAttendeeCollectionResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public MeetingAttendeeCollectionResult(StatusInfo status, IEnumerable<MeetingAttendee> values)
            : base(status, values)
        {
        }

        #endregion
    }
}