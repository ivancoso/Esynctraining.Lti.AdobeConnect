namespace EdugameCloud.Core.Domain.Formats.Edugame
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Edugame question.
    /// </summary>
    [XmlRoot(ElementName = "question")]
    public class EdugameQuestion
    {
        /// <summary>
        /// Gets or sets qustion order.
        /// </summary>
        [XmlAttribute(AttributeName = "order")]
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets score.
        /// </summary>
        [XmlAttribute(AttributeName = "score")]
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets question title.
        /// </summary>
        [XmlElement(ElementName = "instruction")]
        public string Instruction { get; set; }

        /// <summary>
        /// Gets or sets question title.
        /// </summary>
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets question type.
        /// </summary>
        [XmlElement(ElementName = "type", Type = typeof(EdugameQuestionType))]
        public EdugameQuestionType Type { get; set; }

        /// <summary>
        /// Gets or sets question feedback.
        /// </summary>
        [XmlElement(ElementName = "feedback", Type = typeof(EdugameQuestionFeedback))]
        public EdugameQuestionFeedback Feedback { get; set; }

        /// <summary>
        /// Gets or sets image.
        /// </summary>
        [XmlElement(ElementName = "image", IsNullable = true)]
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets image name.
        /// </summary>
        [XmlElement(ElementName = "image-name", IsNullable = true)]
        public string ImageName { get; set; }

        /// <summary>
        /// Gets or sets list of answers.
        /// </summary>
        [XmlElement(ElementName = "distractor", Type = typeof(EdugameDistractor))]
        public List<EdugameDistractor> Distractors { get; set; }
    }
}
