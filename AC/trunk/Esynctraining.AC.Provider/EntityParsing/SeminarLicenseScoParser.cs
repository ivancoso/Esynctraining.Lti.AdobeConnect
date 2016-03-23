using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class SeminarLicenseScoParser
    {
        public static SeminarLicenseSco Parse(XmlNode xml)
        {
            if (xml == null)
            {
                return null;
            }

            return new SeminarLicenseSco
            {
                AclId = xml.SelectSingleNodeValue("acl-id/text()"),
                BeginDate = xml.ParseNodeDateTime("begindate/text()", default(DateTime)),
                EndDate = xml.ParseNodeDateTime("enddate/text()", default(DateTime)),
                DateCreated = xml.ParseNodeDateTime("date-created/text()", DateTime.Now),
                DateModified = xml.ParseNodeDateTime("date-modified/text()", DateTime.Now),
                DispliaySeq = xml.ParseNodeInt("display-seq/text()"),
                FolderId = xml.SelectSingleNodeValue("folder-id/text()"),
                Icon = xml.SelectSingleNodeValue("icon/text()"),
                IsExpired = xml.ParseNodeBool("is-expired/text()"),
                IsFolder = xml.ParseNodeBool("is-folder/text()"),
                IsSeminar = xml.ParseNodeBool("is-seminar/text()"),
                Name = xml.SelectSingleNodeValue("name/text()"),
                Quota = xml.ParseNodeInt("quota/text()"),
                QuotaId = xml.SelectSingleNodeValue("quota-id/text()"),
                ScoId = xml.SelectSingleNodeValue("sco-id/text()"),
                Type = xml.ParseAttributeEnum("type", ScoType.not_set),
                UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
            };
        }

    }

}