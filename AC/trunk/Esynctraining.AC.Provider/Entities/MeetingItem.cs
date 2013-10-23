/*
Copyright (c) 2007-2009 Dmitry Stroganov (DmitryStroganov.info)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// MeetingItem structure
    /// </summary>
    [Serializable]
    [XmlRoot("meeting")]
    public class MeetingItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingItem"/> class.
        /// </summary>
        public MeetingItem()
        {
            this.Type = ScoType.not_set;
        }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the folder id.
        /// </summary>
        [XmlAttribute("folder-id")]
        public string FolderId { get; set; }

        /// <summary>
        /// Gets or sets the active participants.
        /// </summary>
        [XmlAttribute("active-participants")]
        public int ActiveParticipants { get; set; }

        /// <summary>
        /// Gets or sets the meeting name.
        /// </summary>
        [XmlElement("meeting-name")]
        public string MeetingName { get; set; }

        /// <summary>
        /// Gets or sets the meeting description.
        /// </summary>
        [XmlElement("meeting-description")]
        public string MeetingDescription { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [XmlElement("lang")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the SCO tag.
        /// </summary>
        [XmlElement("sco-tag")]
        public string ScoTag { get; set; }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        [XmlElement("domain-name")]
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the url path.
        /// </summary>
        [XmlElement("url-path")]
        public string UrlPath { get; set; }

        /// <summary>
        /// Gets or sets the full url.
        /// </summary>
        public string FullUrl { get; set; }

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
        /// Gets or sets the date modified.
        /// </summary>
        [XmlElement("date-modified")]
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is folder.
        /// </summary>
        [XmlElement("is-folder")]
        public bool IsFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether expired.
        /// </summary>
        [XmlElement("expired")]
        public bool Expired { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [XmlElement("duration")]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the byte count.
        /// </summary>
        [XmlElement("byte-count")]
        public long ByteCount { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [XmlElement]
        [XmlElement("type")]
        public ScoType Type { get; set; }

        /// <summary>
        /// Gets or sets the permission id.
        /// </summary>
        [XmlAttribute("permission-id")]
        public string PermissionId { get; set; }
    }
}
