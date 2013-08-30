namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Shortcut collection parser.
    /// </summary>
    internal static class ShortcutCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//shortcuts/sco";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<ScoShortcut> Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<ScoShortcut>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>()
                .Select(ShortcutParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }

        /// <summary>
        /// The get by type.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="ScoShortcut"/>.
        /// </returns>
        public static ScoShortcut GetByType(XmlNode xml, string type)
        {
            return Parse(xml).FirstOrDefault(s => string.Equals(s.Type, type, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
