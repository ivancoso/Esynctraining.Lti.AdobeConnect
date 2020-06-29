using System;

namespace EdugameCloud.Lti.Domain.Entities
{
    public enum ScheduleDescriptor : byte
    {
        /// <summary>
        /// Checks for signals in AgilixBuzz
        /// </summary>
        [Obsolete("No in use")]
        AgilixBuzzSignals = 0,

        /// <summary>
        /// Checks for LMS sessions to be cleared
        /// </summary>
        CleanLmsSessions = 1,
    }
}