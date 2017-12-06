namespace EdugameCloud.Lti.AgilixBuzz
{
    /// <summary>
    /// The AgilixBuzz course.
    /// </summary>
    internal sealed class Course
    {
        /// <summary>
        /// Gets or sets the course id.
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the course title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the course start date.
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or sets the course end date.
        /// </summary>
        public string EndDate { get; set; }

    }

}