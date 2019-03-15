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

            var item = new ScoInfo();
            Parse(xml, item);
            return item;
        }

        public static void Parse(XmlNode xml, ScoInfo info)
        {
            if (xml == null || xml.Attributes == null)
            {
                return;
            }

            info.AccountId = xml.SelectAttributeValue("account-id");
            info.ScoId = xml.SelectAttributeValue("sco-id");
            info.FolderId = xml.SelectAttributeValue("folder-id");
            info.Icon = xml.SelectAttributeValue("icon");
            info.SourceScoId = xml.SelectAttributeValue("source-sco-id");
            info.Language = xml.SelectAttributeValue("lang");
            info.Type = xml.ParseAttributeEnum("type", ScoType.not_set);

            info.BeginDate = xml.ParseNodeDateTime("date-begin/text()", default(DateTime));

            info.BeginDateOffset = xml.ParseNodeDateTimeOffset("date-begin/text()");

            info.EndDate = xml.ParseNodeDateTime("date-end/text()", default(DateTime));
            info.DateCreated = xml.ParseNodeDateTime("date-created/text()", DateTime.Now);
            info.DateModified = xml.ParseNodeDateTime("date-modified/text()", DateTime.Now);
            info.Name = xml.SelectSingleNodeValue("name/text()");
            info.Description = xml.SelectSingleNodeValue("description/text()");
            info.UrlPath = xml.SelectSingleNodeValue("url-path/text()");
            info.PassingScore = xml.ParseNodeInt("passing-score/text()");
            info.Duration = xml.ParseNodeInt("duration/text()");
            info.SectionCount = xml.ParseNodeInt("section-count/text()");
            info.ExternalUrl = xml.SelectSingleNodeValue("external-url/text()");
            info.MaxScore = xml.ParseNodeInt("max-score/text()");
            info.TelephonyProfile = xml.SelectSingleNodeValue("telephony-profile/text()");
            info.ScoTag = xml.SelectSingleNodeValue("sco-tag/text()");
            info.MeetingPasscode = xml.SelectSingleNodeValue("meeting-passcode/text()");
            info.EventGuestPolicy = xml.SelectSingleNodeValue("event-guest-policy/text()");
            info.UpdateLinkedItem = xml.ParseNodeBool("update-linked-item/text()");
            info.EventTemplateScoId = xml.SelectSingleNodeValue("event-template/text()");
        }

    }

}
