namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    /// The lms quiz info dto.
    /// </summary>
    public class LmsQuizInfoDTO
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        public int course { get; set; }

        /// <summary>
        /// Gets or sets the course name.
        /// </summary>
        public string courseName { get; set; }

        /// <summary>
        /// Gets or sets the last modified lms.
        /// </summary>
        public int lastModifiedLMS { get; set; }

        /// <summary>
        /// Gets or sets the last modified egc.
        /// </summary>
        public int lastModifiedEGC { get; set; }
    }
}
