namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// SCO Info.
    /// </summary>
    [Serializable]
    [XmlRoot("row")]
    public class QuizResponse
    {
        /// <summary>
        /// Gets or sets the transcript id.
        /// </summary>
        [XmlAttribute("transcript-id")]
        public string TranscriptId { get; set; }

        /// <summary>
        /// Gets or sets the interaction id.
        /// </summary>
        [XmlAttribute("interaction-id")]
        public string InteractionId { get; set; }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the folder id.
        /// </summary>
        [XmlAttribute("score")]
        public string Score { get; set; }

        /// <summary>
        /// Gets or sets the display-seq in quiz.
        /// </summary>
        [XmlAttribute("display-seq")]
        public string DisplaySeq { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [XmlElement("date-created")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("sco-name")]
        public string ScoName { get; set; }

        /// <summary>
        /// Gets or sets the respondent user name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description (the quiz question name).
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlElement("response")]
        public string Response { get; set; }
    }
}
