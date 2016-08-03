namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot("curriculum")]
    public class CurriculumUpdateItem : MeetingUpdateItem
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the content source sco icon.
        /// </summary>
        [XmlAttribute("content-source-sco-icon")]
        public string ContentSourceScoIcon { get; set; }

        /// <summary>
        /// Gets or sets the content source sco icon.
        /// </summary>
        [XmlAttribute("external-url")]
        public string ExternalUrl { get; set; }

        /// <summary>
        /// Gets or sets the display sequence.
        /// </summary>
        [XmlAttribute("display-seq")]
        public string DisplaySeq { get; set; }

        /// <summary>
        /// Gets or sets the source sco icon.
        /// </summary>
        [XmlAttribute("source-sco-icon")]
        public string SourceScoIcon { get; set; }

        /// <summary>
        /// Gets or sets the source sco type.
        /// </summary>
        [XmlAttribute("source-sco-type")]
        public string SourceScoType { get; set; }

        #endregion
    }
}