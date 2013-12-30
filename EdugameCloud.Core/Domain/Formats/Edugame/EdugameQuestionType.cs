namespace EdugameCloud.Core.Domain.Formats.Edugame
{
    using System.Xml.Serialization;

    /// <summary>
    /// Edugame question type.
    /// </summary>
    [XmlRoot(ElementName = "type")]
    public class EdugameQuestionType
    {
        /// <summary>
        /// Gets or sets question type id.
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets question type description.
        /// </summary>
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
    }
}
