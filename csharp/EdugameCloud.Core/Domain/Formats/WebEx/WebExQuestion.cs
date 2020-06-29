namespace EdugameCloud.Core.Domain.Formats.WebEx
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// WebEx question.
    /// </summary>
    [XmlRoot(ElementName = "QUESTION")]
    public class WebExQuestion
    {
        /// <summary>
        /// Gets or sets question type.
        /// </summary>
        [XmlAttribute(AttributeName = "TYPE")]
        public WebExQuestionType Type { get; set; }

        /// <summary>
        /// Gets or sets question title.
        /// </summary>
        [XmlAttribute(AttributeName = "TITLE")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets list of answers.
        /// </summary>
        [XmlElement(ElementName = "ANSWER", Type = typeof(WebExAnswer))]
        public List<WebExAnswer> Answers { get; set; }
    }
}