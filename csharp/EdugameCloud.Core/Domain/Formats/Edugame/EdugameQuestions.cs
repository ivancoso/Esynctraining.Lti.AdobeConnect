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
            this.Questions = new List<EdugameQuestion>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdugameQuestions"/> class.
        /// </summary>
        /// <param name="subModuleId">
        /// The smi Type Id.
        /// </param>
        /// <param name="questions">
        /// Edugame questions.
        /// </param>
        public EdugameQuestions(int subModuleId, IEnumerable<EdugameQuestion> questions)
            : this()
        {
            this.Questions.AddRange(questions);
            this.SubModuleId = subModuleId;
        }

        /// <summary>
        /// Gets or sers smi type Id.
        /// </summary>
        [XmlAttribute(AttributeName = "subModuleId")]
        public int SubModuleId { get; set; }

        /// <summary>
        /// Gets or sets questions.
        /// </summary>
        [XmlElement(ElementName = "question")]
        public List<EdugameQuestion> Questions { get; set; }
    }
}
