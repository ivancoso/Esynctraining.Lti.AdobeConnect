using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;
using Esynctraining.AC.Provider.Utils;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class ReportUserTrainingsTakenContentParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>SCO Content.</returns>
        public static UserTrainingsTaken Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new UserTrainingsTaken()
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    Type = xml.SelectAttributeValue("type"),
                    Attempts = xml.ParseAttributeInt("attempts"),
                    FromCurriculum = xml.ParseNodeBool("from-curriculum/text()"),
                    Certificate = xml.SelectAttributeValue("certificate"),
                    Name= xml.SelectSingleNodeValue("name/text()"),
                    Description = xml.SelectSingleNodeValue("description/text()"),
                    Icon = xml.SelectAttributeValue("icon"),
                    MaxRetries = xml.SelectAttributeValue("max-retries"),
                    Status = xml.SelectAttributeValue("status"),
                    Score = xml.ParseAttributeInt("score"),
                    MaxScore = xml.ParseAttributeInt("max-score"),
                    TranscriptId = xml.SelectAttributeValue("transcript-id"),
                    DateTaken = xml.ParseNodeDateTimeLocal("date-taken/text()", default(DateTime)),
                    ScoTag = xml.SelectSingleNodeValue("sco-tag/text()"),
                    UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                    //PermissionId = EnumReflector.ReflectEnum(xml.SelectAttributeValue("permission-id"), PermissionId.none)
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