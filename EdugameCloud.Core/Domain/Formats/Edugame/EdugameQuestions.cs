namespace EdugameCloud.Core.Domain.Formats.Edugame
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Edugame questions.
    /// </summary>
    [XmlRoot(ElementName = "questions")]
    public class EdugameQuestions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdugameQuestions"/> class.
        /// </summary>
        public EdugameQuestions()
        {
            Questions = new List<EdugameQuestion>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdugameQuestions"/> class.
        /// </summary>
        /// <param name="smiId">SubModule item Id.</param>
        /// <param name="questions">Edugame questions.</param>
        public EdugameQuestions(int smiId, IEnumerable<EdugameQuestion> questions) : this()
        {
            Questions.AddRange(questions);
            SmiId = smiId;
        }

        /// <summary>
        /// Gets or sers smiId.
        /// </summary>
        [XmlAttribute(AttributeName = "smiId")]
        public int SmiId { get; set; }

        /// <summary>
        /// Gets or sets questions.
        /// </summary>
        [XmlElement(ElementName = "question")]
        public List<EdugameQuestion> Questions { get; set; }
    }
}
