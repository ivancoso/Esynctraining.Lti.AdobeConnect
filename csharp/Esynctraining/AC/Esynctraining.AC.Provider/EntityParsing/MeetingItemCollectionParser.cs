namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The meeting item collection parser.
    /// </summary>
    internal static class MeetingItemCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        //private const string Path = "//report-bulk-objects/row";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="adobeConnectUrl">AdobeConnect Root Url.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<MeetingItem> Parse(XmlNode xml, string path)
        {
            //path = path ?? Path;
            if (xml == null || !xml.NodeListExists(path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", path));

                return Enumerable.Empty<MeetingItem>();
            }

            return xml.SelectNodes(path).Cast<XmlNode>()
                .Select(node => MeetingItemParser.Parse(node))
                .Where(item => item != null)
                .ToArray();
        }
    }
}
