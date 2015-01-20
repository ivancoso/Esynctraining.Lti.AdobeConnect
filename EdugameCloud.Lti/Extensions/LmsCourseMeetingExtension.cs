namespace EdugameCloud.Lti.Extensions
{
    using EdugameCloud.Lti.Domain.Entities;

    /// <summary>
    /// The lms course meeting extension.
    /// </summary>
    public static class LmsCourseMeetingExtension
    {
        /// <summary>
        /// The meeting sco id.
        /// </summary>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetMeetingScoId(this LmsCourseMeeting meeting)
        {
            if (meeting.ScoId != null)
            {
                return meeting.ScoId;
            }
            if (meeting.OfficeHours != null)
            {
                return meeting.OfficeHours.ScoId;
            }
            return null;
        }
    }
}
