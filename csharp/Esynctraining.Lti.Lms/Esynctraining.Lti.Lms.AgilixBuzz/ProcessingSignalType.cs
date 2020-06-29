namespace Esynctraining.Lti.Lms.AgilixBuzz
{
    /// <summary>
    /// The parsed agilixBuzz signal.
    /// </summary>
    internal enum ProcessingSignalType
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
        Skip = 3,

    }
}
