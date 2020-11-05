namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    ///     Curriculum taker.
    /// </summary>
    public class CurriculumTaker : CurriculumContent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        [XmlElement("access")]
        public string Access { get; set; }

        /// <summary>
        /// Gets or sets the asset id.
        /// </summary>
        [XmlAttribute("asset-id")]
        public string AssetId { get; set; }

        /// <summary>
        /// Gets or sets the attempts.
        /// </summary>
        [XmlAttribute("attempts")]
        public int Attempts { get; set; }

        /// <summary>
        /// Gets or sets the certificate.
        /// </summary>
        [XmlAttribute("certificate")]
        public string Certificate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether credit granted.
        /// </summary>
        [XmlElement("credit-granted")]
        public bool CreditGranted { get; set; }

        /// <summary>
        /// Gets or sets the date taken.
        /// </summary>
        [XmlElement("date-taken")]
        public DateTime DateTaken { get; set; }

        /// <summary>
        /// Gets or sets the external url.
        /// </summary>
        [XmlElement("external-url")]
        public string ExternalUrl { get; set; }

        /// <summary>
        /// Gets or sets the max retries.
        /// </summary>
        [XmlAttribute("max-retries")]
        public int MaxRetries { get; set; }

        /// <summary>
        /// Gets or sets the max score.
        /// </summary>
        [XmlAttribute("max-score")]
        public int MaxScore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether override.
        /// </summary>
        [XmlElement("override")]
        public bool Override { get; set; }

        /// <summary>
        /// Gets or sets the path type.
        /// </summary>
        [XmlAttribute("path-type")]
        public string PathType { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        [XmlAttribute("score")]
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [XmlAttribute("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the transcript id.
        /// </summary>
        [XmlAttribute("transcript-id")]
        public string TranscriptId { get; set; }

        #endregion
    }
}