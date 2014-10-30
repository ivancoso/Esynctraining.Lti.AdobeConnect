namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Curriculum Taker parser.
    /// </summary>
    internal static class CurriculumTakerParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>SCO Content.</returns>
        public static CurriculumTaker Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {

                return new CurriculumTaker
                           {
                               Depth = xml.ParseAttributeInt("depth"),
                               Lang = xml.SelectAttributeValue("lang"),
                               ContentSourceScoIcon = xml.SelectAttributeValue("content-source-sco-icon"),
                               SourceScoIcon = xml.SelectAttributeValue("source-sco-icon"),
                               SourceScoType = xml.SelectAttributeValue("source-sco-type"),
                               ScoId = xml.SelectAttributeValue("sco-id"),
                               SourceScoId = xml.SelectAttributeValue("source-sco-id"),
                               FolderId = xml.ParseAttributeInt("folder-id"),
                               Type = xml.SelectAttributeValue("type"),
                               Icon = xml.SelectAttributeValue("icon"),
                               DisplaySequence = xml.ParseAttributeInt("display-seq"),
                               Duration = xml.ParseAttributeInt("duration"),
                               IsFolder = xml.ParseAttributeInt("is-folder") != 0,
                               Name = xml.SelectSingleNodeValue("name/text()"),
                               Description = xml.SelectSingleNodeValue("description/text()"),
                               UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                               BeginDate = xml.ParseNodeDateTime("date-begin/text()", default(DateTime)),
                               EndDate = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                               DateCreated = xml.ParseNodeDateTime("date-created/text()", DateTime.Now),
                               DateModified = xml.ParseNodeDateTime("date-modified/text()", DateTime.Now),
                               IsSeminar = xml.ParseNodeBool("is-seminar/text()"),
                               Access = xml.SelectSingleNodeValue("access/text()"),
                               AssetId = xml.SelectAttributeValue("asset-id"),
                               Attempts = xml.ParseAttributeInt("attempts"),
                               Certificate = xml.SelectAttributeValue("certificate"),
                               CreditGranted = xml.ParseNodeBool("credit-granted/text()"),
                               DateTaken = xml.ParseNodeDateTime("date-taken/text()", DateTime.Now),
                               ExternalUrl = xml.SelectSingleNodeValue("external-url/text()"),
                               MaxRetries = xml.ParseAttributeInt("max-retries"),
                               MaxScore = xml.ParseAttributeInt("max-score"),
                               Override = xml.ParseNodeBool("override"),
                               PathType = xml.SelectAttributeValue("path-type"),
                               Status = xml.SelectAttributeValue("status"),
                               Score = xml.ParseAttributeInt("score"),
                               TranscriptId = xml.SelectAttributeValue("transcript-id"),
                           };
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
