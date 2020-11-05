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
    internal static class ScoSearchByFieldParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//sco-search-by-field/sco";

        /// <summary>
        /// The path.
        /// </summary>
        private const string Path2 = "//sco-search-by-field-info/sco";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<ScoContent> Parse(XmlNode xml)
        {
            if (xml == null || (!xml.NodeListExists(Path) && !xml.NodeListExists(Path2)))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<ScoContent>();
            }

            return (xml.NodeListExists(Path) ? xml.SelectNodes(Path) : xml.SelectNodes(Path2)).Cast<XmlNode>()
                .Select(ScoContentParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}
