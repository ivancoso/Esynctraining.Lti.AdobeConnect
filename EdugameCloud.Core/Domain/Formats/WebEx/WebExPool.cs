namespace EdugameCloud.Core.Domain.Formats.WebEx
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// WebEx pool.
    /// </summary>
    [XmlRoot(ElementName = "POLL")]
    public class WebExPool
    {
        /// <summary>
        /// Gets or sets the value that indicates pool type.
        /// </summary>
        [XmlAttribute(AttributeName = "TYPE")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether to show timer.
        /// </summary>
        [XmlAttribute(AttributeName = "SHOWTIMER")]
        public HumalBool ShowTimer { get; set; }

        /// <summary>
        /// Gets or sets alarm value.
        /// </summary>
        [XmlAttribute(AttributeName = "ALARM")]
        public string Alarm { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether there is no answer.
        /// </summary>
        [XmlAttribute(AttributeName = "NOANSWER")]
        public HumalBool NoAnswer { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether to show response.
        /// </summary>
        [XmlAttribute(AttributeName = "SHOWRESPONSE")]
        public HumalBool ShowResponse { get; set; }

        /// <summary>
        /// Gets or sets list of questions.
        /// </summary>
        [XmlElement(ElementName = "QUESTION", Type = typeof(WebExQuestion))]
        public List<WebExQuestion> Questions { get; set; }
    }
}