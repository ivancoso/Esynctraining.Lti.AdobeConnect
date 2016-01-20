namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;
    using Esynctraining.AC.Provider.Utils;

    /// <summary>
    /// The meeting item parser.
    /// </summary>
    internal static class MeetingItemParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="serviceUrl">The service URL.</param>
        /// <returns>Meeting Item or null if it's a folder.</returns>
        public static MeetingItem Parse(XmlNode xml, string serviceUrl)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                var item = new MeetingItem
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    Type = EnumReflector.ReflectEnum(xml.SelectAttributeValue("type"), ScoType.not_set),
                    MeetingName = xml.SelectSingleNodeValue("name/text()"),
                    UrlPath = xml.SelectSingleNodeValue("url/text()"),

                    // NOTE: if folder =>  date-begin is null
                    DateBegin = xml.ParseNodeDateTime("date-begin/text()", default(DateTime)),
                    DateEnd = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                    DateModified = xml.ParseNodeDateTime("date-modified/text()", default(DateTime)),
                    PermissionId = xml.SelectAttributeValue("permission-id")
                };

                if (string.IsNullOrEmpty(item.UrlPath))
                {
                    item.UrlPath = xml.SelectSingleNodeValue("url-path/text()");
                }

                if (!string.IsNullOrEmpty(item.UrlPath) && !string.IsNullOrEmpty(serviceUrl))
                {
                    var u = new Uri(serviceUrl);

                    item.FullUrl = "https://" + u.GetComponents(UriComponents.Host, UriFormat.SafeUnescaped) + item.UrlPath;
                }

                item.Duration = item.DateEnd.Subtract(item.DateBegin);

                // if item.DateBegin is not defined and duration is 0 => then this is the folder which should be ignored
                if (!item.DateBegin.Equals(default(DateTime)) || item.Duration.TotalMinutes != 0)
                {
                    return item;
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
