namespace EdugameCloud.Core.Domain.Formats.Edugame
{
    using System.Xml.Serialization;

    /// <summary>
    /// Edugame question result.
    /// </summary>
    [XmlRoot(ElementName = "feedback")]
    public class EdugameQuestionFeedback
    {
        /// <summary>
        /// Gets or sets correct feedback.
        /// </summary>
        [XmlElement(ElementName = "correct")]
        public string Correct { get; set; }

        /// <summary>
        /// Gets or sets incorrect feedback.
        /// </summary>
        [XmlElement(ElementName = "incorrect")]
        public string Incorrect { get; set; }

        /// <summary>
        /// Gets or sets question title.
        /// </summary>
        [XmlElement(ElementName = "correctReference", IsNullable = true)]
        public string CorrectReference { get; set; }

        /// <summary>
        /// Gets or sets question title.
        /// </summary>
        [XmlElement(ElementName = "hint", IsNullable = true)]
        public string Hint { get; set; }
    }
}
