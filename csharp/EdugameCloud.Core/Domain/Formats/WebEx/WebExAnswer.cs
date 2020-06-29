namespace EdugameCloud.Core.Domain.Formats.WebEx
{
    using System.Xml.Serialization;

    /// <summary>
    /// WebEx answer.
    /// </summary>
    [XmlRoot(ElementName = "ANSWER")]
    public class WebExAnswer
    {
        /// <summary>
        /// Gets or sets the value that indicates whether answer is correct.
        /// </summary>
        [XmlAttribute(AttributeName = "ISCORRECT")]
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Gets or sets answer text.
        /// </summary>
        [XmlText]
        public string Text { get; set; }
    }
}