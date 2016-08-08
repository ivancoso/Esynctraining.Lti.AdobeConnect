namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The User collection parser.
    /// </summary>
    internal static class UserCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//report-bulk-users/row";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of User Items.</returns>
        public static IEnumerable<User> Parse(XmlNode xml, string path = Path)
        {
            if (xml == null || !xml.NodeListExists(path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", path));

                return Enumerable.Empty<User>();
            }

            return xml.SelectNodes(path).Cast<XmlNode>()
                .Select(node => UserParser.Parse(node))
                .Where(item => item != null)
                .ToArray();
        }

    }

}
