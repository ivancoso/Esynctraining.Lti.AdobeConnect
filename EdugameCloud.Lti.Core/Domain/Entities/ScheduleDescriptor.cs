using System;

namespace EdugameCloud.Lti.Domain.Entities
{
    public enum ScheduleDescriptor : byte
    {
        /// <summary>
        /// Checks for signals in brain honey
        /// </summary>
        [Obsolete("No in use")]
        BrainHoneySignals = 0,

        /// <summary>
        /// Checks for LMS sessions to be cleared
        /// </summary>
        CleanLmsSessions = 1,
    }
}