namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The meeting item collection parser.
    /// </summary>
    internal static class ScoContentCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//scos/sco";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<ScoContent> Parse(XmlNode xml, string path = null)
        {
            if (xml == null || !xml.NodeListExists(path ?? Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", path ?? Path));

                return Enumerable.Empty<ScoContent>();
            }

            return xml.SelectNodes(path ?? Path).Cast<XmlNode>()
                .Select(ScoContentParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}
