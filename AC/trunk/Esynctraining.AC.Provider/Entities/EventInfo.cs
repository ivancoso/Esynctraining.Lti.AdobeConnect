using System.Collections.Generic;

namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Event information 
    /// </summary>
    public class EventInfo
    {
        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the tree id.
        /// </summary>
        [XmlAttribute("tree-id")]
        public long TreeId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        [XmlElement("domain-name")]
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the date begin.
        /// </summary>
        [XmlElement("date-begin")]
        public DateTime DateBegin { get; set; }

        /// <summary>
        /// Gets or sets the date end.
        /// </summary>
        [XmlElement("date-end")]
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Gets or sets the url path.
        /// </summary>
        [XmlElement("url-path")]
        public string UrlPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether expired.
        /// </summary>
        [XmlElement("expired")]
        public bool Expired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Register Without Password.
        /// </summary>
        [XmlElement("password-bypass")]
        public bool PasswordBypass { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [XmlElement("duration")]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the event-info.
        /// </summary>
        [XmlElement("event-info")]
        public string Info { get; set; }

        /// <summary>
        /// Gets or sets the speaker-name.
        /// </summary>
        [XmlElement("speaker-name")]
        public string SpeakerName { get; set; }

        /// <summary>
        /// Gets or sets the speaker-brief-overview.
        /// </summary>
        [XmlElement("speaker-brief-overview")]
        public string SpeakerBriefOverview { get; set; }

        /// <summary>
        /// Gets or sets the speaker-detailed-overview.
        /// </summary>
        [XmlElement("speaker-detailed-overview")]
        public string SpeakerDetailedOverview { get; set; }

        /// <summary>
        /// Gets or sets the speaker-image.
        /// </summary>
        [XmlElement("speaker-image")]
        public string SpeakerImage { get; set; }
    }

    public class EventRegistrationDetails 
    {
        public IEnumerable<EventField> EventFields { get; set; }
        public IEnumerable<EventUserField> UserFields { get; set; }

        //UserInfo - implement if necessary

    }

    public class EventFieldBase
    {
        public string InteractionType { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; }
        public string InputDataType { get; set; }
        public string Description { get; set; }
    }

    public class EventField : EventFieldBase
    {
        public string InteractionId { get; set; }
    }

    public class EventUserField : EventFieldBase
    {
        public string Name { get; set; }
        public string Response { get; set; }
    }

}
