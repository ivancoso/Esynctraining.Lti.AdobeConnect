namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The SCO Info parser.
    /// </summary>
    internal static class ScoInfoParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>SCO Info.</returns>
        public static ScoInfo Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new ScoInfo
            {
                AccountId = xml.SelectAttributeValue("account-id"),
                ScoId = xml.SelectAttributeValue("sco-id"),
                FolderId = xml.SelectAttributeValue("folder-id"),
                Icon = xml.SelectAttributeValue("icon"),
                SourceScoId = xml.SelectAttributeValue("source-sco-id"),
                Language = xml.SelectAttributeValue("lang"),
                Type = xml.ParseAttributeEnum("type", ScoType.not_set),
                BeginDate = xml.ParseNodeDateTime("date-begin/text()", default(DateTime)),
                EndDate = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                DateCreated = xml.ParseNodeDateTime("date-created/text()", DateTime.Now),
                DateModified = xml.ParseNodeDateTime("date-modified/text()", DateTime.Now),
                Name = xml.SelectSingleNodeValue("name/text()"),
                Description = xml.SelectSingleNodeValue("description/text()"),
                UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                PassingScore = xml.ParseNodeInt("passing-score/text()"),
                Duration = xml.ParseNodeInt("duration/text()"),
                SectionCount = xml.ParseNodeInt("section-count/text()"),
                ExternalUrl = xml.SelectSingleNodeValue("external-url/text()"),
                MaxScore = xml.ParseNodeInt("max-score/text()"),
            };
        }

    }

}
