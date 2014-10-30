namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    ///     Curriculum Content.
    /// </summary>
    [Serializable]
    public class CurriculumContent : ScoContent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the content source SCO icon.
        /// </summary>
        [XmlAttribute("content-source-sco-icon")]
        public string ContentSourceScoIcon { get; set; }

        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        [XmlAttribute("depth")]
        public int Depth { get; set; }

        /// <summary>
        /// Gets or sets the source SCO icon.
        /// </summary>
        [XmlAttribute("source-sco-icon")]
        public string SourceScoIcon { get; set; }

        /// <summary>
        /// Gets or sets the source SCO type.
        /// </summary>
        [XmlAttribute("source-sco-type")]
        public string SourceScoType { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [XmlAttribute("lang")]
        public string Lang { get; set; }

        /// <summary>
        /// The is meeting.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsMeeting()
        {
            return this.Icon == "virtual-classroom" || this.Icon == "meeting";
        }

        #endregion
    }
}