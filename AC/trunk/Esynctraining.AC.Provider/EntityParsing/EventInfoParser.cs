namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;
    using Esynctraining.AC.Provider.Utils;

    /// <summary>
    /// The Event Info parser.
    /// </summary>
    internal static class EventInfoParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>EventInfo object.</returns>
        public static EventInfo Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                var ei = new EventInfo
                             {
                                 ScoId = xml.SelectAttributeValue("sco-id"),
                                 PermissionId = EnumReflector.ReflectEnum(xml.SelectAttributeValue("permission-id"), PermissionId.none),
                                 Name = xml.SelectSingleNodeValue("name/text()"),
                                 DomainName = xml.SelectSingleNodeValue("domain-name/text()"),
                                 UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                                 DateBegin = xml.ParseNodeDateTime("date-begin/text()", default(DateTime)),
                                 DateEnd = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                                 Expired = xml.ParseNodeBool("expired/text()")
                             };

                ei.Duration = ei.DateEnd.Subtract(ei.DateBegin);

                // if mDetail.DateBegin is not defined and duration is 0 => then this is the folder which should be ignored
                if (!ei.DateBegin.Equals(default(DateTime)) || ei.Duration.TotalMinutes != 0)
                {
                    return ei;
                }
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
