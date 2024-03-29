﻿namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Curriculum Content parser.
    /// </summary>
    internal static class CurriculumContentParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>SCO Content.</returns>
        public static CurriculumContent Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {

                return new CurriculumContent
                           {
                               Depth = xml.ParseAttributeInt("depth"),
                               Lang = xml.SelectAttributeValue("lang"),
                               ContentSourceScoIcon =
                                   xml.SelectAttributeValue("content-source-sco-icon"),
                               SourceScoIcon = xml.SelectAttributeValue("source-sco-icon"),
                               SourceScoType = xml.SelectAttributeValue("source-sco-type"),
                               ScoId = xml.SelectAttributeValue("sco-id"),
                               SourceScoId = xml.SelectAttributeValue("source-sco-id"),
                               FolderId = xml.ParseAttributeLong("folder-id"),
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
                               DateModified =
                                   xml.ParseNodeDateTime("date-modified/text()", DateTime.Now),
                               IsSeminar = xml.ParseNodeBool("is-seminar/text()", default(bool)),
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
