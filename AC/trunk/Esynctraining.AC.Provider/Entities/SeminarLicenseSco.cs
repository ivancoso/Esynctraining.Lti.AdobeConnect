using System;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    [XmlRoot("sco")]
    public class SeminarLicenseSco
    {
        public SeminarLicenseSco()
        {
            this.Type = ScoType.not_set;
        }

        [XmlElement("acl-id")]
        public string AclId { get; set; }

        [XmlElement("begindate")]
        public DateTime? BeginDate { get; set; }

        [XmlElement("enddate")]
        public DateTime? EndDate { get; set; }

        [XmlElement("date-created")]
        public DateTime DateCreated { get; set; }

        [XmlElement("date-modified")]
        public DateTime DateModified { get; set; }

        [XmlElement("display-seq")]
        public int DispliaySeq { get; set; }

        [XmlElement("folder-id")]
        public string FolderId { get; set; }

        [XmlElement("icon")]
        public string Icon { get; set; }

        [XmlElement("is-expired")]
        public bool IsExpired { get; set; }

        [XmlElement("is-folder")]
        public bool IsFolder { get; set; }

        [XmlElement("is-seminar")]
        public bool IsSeminar { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("quota")]
        public int? Quota { get; set; }

        [XmlElement("quota-id")]
        public string QuotaId { get; set; }

        [XmlElement("sco-id")]
        public string ScoId { get; set; }

        [XmlElement("type")]
        public ScoType Type { get; set; }

        [XmlElement("url-path")]
        public string UrlPath { get; set; }
    }
}