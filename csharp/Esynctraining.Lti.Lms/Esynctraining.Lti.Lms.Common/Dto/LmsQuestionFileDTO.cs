namespace Esynctraining.Lti.Lms.Common.Dto
{
    /// <summary>
    /// The lms file dto.
    /// </summary>
    public class LmsQuestionFileDTO
    {
        /// <summary>
        /// Gets or sets the file url.
        /// </summary>
        public string fileUrl { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int height { get; set; }

        public string base64Content { get; set; }
    }
}
