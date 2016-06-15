using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class ReportScoViewContentParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>SCO Content.</returns>
        public static ReportScoViewContent Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new ReportScoViewContent()
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    Type = xml.SelectAttributeValue("type"),
                    IsFolder = xml.ParseAttributeInt("is-folder") != 0,
                    Views = xml.ParseAttributeInt("views"),
                    LastViewedDate = xml.ParseNodeDateTimeLocal("last-viewed-date/text()", default(DateTime))
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