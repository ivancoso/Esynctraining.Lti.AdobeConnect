namespace EdugameCloud.Lti.DTO
{
    /// <summary>
    /// The bb assessment dto.
    /// </summary>
    public class BBAssessmentDTO
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Gets or sets the instructions.
        /// </summary>
        public string instructions { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        public BBQuestionDTO[] questions { get; set; }

        public object images { get; set; }
        
    }
}
