namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Event Info parser.
    /// </summary>
    internal static class EventCollectionItemParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>EventInfo object.</returns>
        public static EventCollectionItem Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                var item = new EventCollectionItem
                {
                    ScoId = xml.SelectAttributeValue("sco-id"),
                    Name = xml.SelectSingleNodeValue("name/text()"),
                    UrlPath = xml.SelectSingleNodeValue("url-path/text()"),
                    DateBegin = xml.ParseNodeDateTime("date-begin/text()", default(DateTime)),
                    DateEnd = xml.ParseNodeDateTime("date-end/text()", default(DateTime)),
                };

                item.Duration = item.DateEnd.Subtract(item.DateBegin);

                return item;
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
