namespace EdugameCloud.Lti.API.BrainHoney
{
    /// <summary>
    /// The parsed brain honey signal.
    /// </summary>
    public enum ProcessingSignalType
    {
        /// <summary>
        /// The to process.
        /// </summary>
        CourseToProcess = 0, 

        /// <summary>
        /// The to delete.
        /// </summary>
        CourseToDelete = 1, 

        /// <summary>
        /// The sole enrollment.
        /// </summary>
        SoleEnrollment = 2,

        /// <summary>
        /// Skip this.
        /// </summary>
        Skip = 3
    }
}