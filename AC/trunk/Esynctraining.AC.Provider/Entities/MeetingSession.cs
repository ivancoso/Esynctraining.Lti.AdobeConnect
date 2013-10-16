namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// MeetingItem structure
    /// </summary>
    [Serializable]
    [XmlRoot("meeting-session")]
    public class MeetingSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingSession"/> class.
        /// </summary>
        public MeetingSession()
        {
        }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the version (index).
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the number of participants.
        /// </summary>
        [XmlAttribute("num-participants")]
        public string ParticipantsCount { get; set; }

        /// <summary>
        /// Gets or sets the asset(session) id.
        /// </summary>
        [XmlAttribute("asset-id")]
        public string AssetId { get; set; }

        /// <summary>
        /// Gets or sets the date start.
        /// </summary>
        [XmlElement("date-created")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date end.
        /// </summary>
        [XmlElement("date-end")]
        public DateTime DateEnd { get; set; }
    }
}
