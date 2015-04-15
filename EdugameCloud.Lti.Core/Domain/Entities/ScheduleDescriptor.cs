namespace EdugameCloud.Lti.Domain.Entities
{
    /// <summary>
    /// The schedule descriptor.
    /// </summary>
    public enum ScheduleDescriptor : byte
    {
        /// <summary>
        /// Checks for signals in brain honey
        /// </summary>
        BrainHoneySignals = 0,

        /// <summary>
        /// Checks for LMS sessions to be cleared
        /// </summary>
        CleanLmsSessions = 1,
    }
}