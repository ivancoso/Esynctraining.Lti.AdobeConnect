namespace EdugameCloud.Core.Domain.DTO
{
    /// <summary>
    /// The answer DTO.
    /// </summary>
    public class AnswerDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the answer_match_left.
        /// </summary>
        public string answer_match_left { get; set; }

        /// <summary>
        /// Gets or sets the answer_match_right.
        /// </summary>
        public string answer_match_right { get; set; }

        /// <summary>
        /// Gets or sets the answer_text.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the answer_weight.
        /// </summary>
        public int weight { get; set; }

        /// <summary>
        /// Gets or sets the exact.
        /// </summary>
        public string exact { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int id { get; set; }

        #endregion
    }
}