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
    }
}
