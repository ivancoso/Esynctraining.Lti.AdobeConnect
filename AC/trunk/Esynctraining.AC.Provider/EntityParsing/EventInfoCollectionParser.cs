namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The event item collection parser.
    /// </summary>
    internal static class EventInfoCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//my-events/event";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<EventInfo> Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<EventInfo>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>()
                .Select(EventInfoParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}
