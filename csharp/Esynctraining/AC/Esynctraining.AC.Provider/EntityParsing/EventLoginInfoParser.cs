namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Event Login Info parser.
    /// </summary>
    internal static class EventLoginInfoParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>EventLoginInfo object.</returns>
        public static EventLoginInfo Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                var ei = new EventLoginInfo
                {
                    EntryUrl = xml.SelectSingleNodeValue("entry-url/text()"),
                };

                return ei;
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}

