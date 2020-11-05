namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// response object for report-my-meetings api request
    /// <meeting sco-id="28104" type="meeting" icon="meeting" permission-id="host" active-participants="0">
    ///     <name>Sergey test meeting</name>
    ///     <description>erfvr</description>
    ///     <domain-name>connectdev.esynctraining.com</domain-name>
    ///     <url-path>/r86x3iw2ij4/</url-path>
    ///     <date-begin>2015-12-11T02:00:00.000-08:00</date-begin>
    ///     <date-end>2015-12-11T03:00:00.000-08:00</date-end>
    ///     <expired>true</expired>
    ///     <duration>01:00:00.000</duration>
    /// </meeting>
    /// todo: separate report-bulk-objects response
    /// </summary>
    [XmlRoot("meeting")]
    public class MeetingItem
    {
        public MeetingItem()
        {
            this.Type = ScoType.not_set;
        }

        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }
        [XmlElement("type")]
        public ScoType Type { get; set; }
        [XmlElement("icon")]
        public ScoIcon Icon { get; set; }
        [XmlAttribute("permission-id")]
        public string PermissionId { get; set; }
        [XmlAttribute("active-participants")]
        public int ActiveParticipants { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("description")]
        public string Description { get; set; }
        [XmlElement("domain-name")]
        public string DomainName { get; set; }
        [XmlElement("url-path")]
        public string UrlPath { get; set; }
        [XmlElement("date-begin")]
        public DateTime DateBegin { get; set; }
        [XmlElement("date-end")]
        public DateTime DateEnd { get; set; }
        [XmlElement("expired")]
        public bool Expired { get; set; }
        [XmlElement("duration")]
        public TimeSpan Duration { get; set; }

        //not used:
        /*
        [XmlAttribute("folder-id")]
        public string FolderId { get; set; }

        [XmlElement("meeting-name")]
        public string MeetingName { get; set; }

        [XmlElement("meeting-description")]
        public string MeetingDescription { get; set; }

        [XmlElement("lang")]
        public string Language { get; set; }

        [XmlElement("sco-tag")]
        public string ScoTag { get; set; }

        public string FullUrl { get; set; }

        [XmlElement("date-modified")]
        public DateTime DateModified { get; set; }

        [XmlElement("is-folder")]
        public bool IsFolder { get; set; }

        [XmlElement("byte-count")]
        public long ByteCount { get; set; }
        */
    }
}
