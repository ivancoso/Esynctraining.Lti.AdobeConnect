namespace EdugameCloud.Lti.Extensions
{
    using System;
    using EdugameCloud.Lti.Domain.Entities;

    /// <summary>
    /// The LMS course meeting extension.
    /// </summary>
    public static class LmsCourseMeetingExtension
    {
        /// <summary>
        /// The meeting SCO id.
        /// </summary>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetMeetingScoId(this LmsCourseMeeting meeting)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));

            if (meeting.ScoId != null)
            {
                return meeting.ScoId;
            }

            if (meeting.OfficeHours != null)
            {
                return meeting.OfficeHours.ScoId;
            }

            // TODO: investigate
            //if (meeting.Id > 0)
            //    throw new InvalidOperationException($"sco-id for meeting {meeting.Id} not found.");

            return null;
        }

    }

}
