namespace EdugameCloud.Core.Domain.Formats.Edugame
{
    using System.Xml.Serialization;

    /// <summary>
    /// Edugame distractor.
    /// </summary>
    [XmlRoot(ElementName = "distractor")]
    public class EdugameDistractor
    {
        /// <summary>
        /// Gets or sets a value indicating whether is correct.
        /// </summary>
        [XmlAttribute(AttributeName = "isCorrect")]
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Gets or sets the distractor order.
        /// </summary>
        [XmlAttribute(AttributeName = "order")]
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets image.
        /// </summary>
        [XmlElement(ElementName = "image", IsNullable = true)]
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets image name.
        /// </summary>
        [XmlElement(ElementName = "imageName", IsNullable = true)]
        public string ImageName { get; set; }

        /// <summary>
        /// Gets or sets the distractor order.
        /// </summary>
        [XmlAttribute(AttributeName = "distractorType")]
        public int DistractorType { get; set; }
        
        /// <summary>
        /// Gets or sets the distractor text.
        /// </summary>
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
    }
}
