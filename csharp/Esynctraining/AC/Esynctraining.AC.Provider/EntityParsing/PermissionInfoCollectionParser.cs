namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The permission item collection parser.
    /// </summary>
    internal static class PermissionInfoCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string PathPrincipals = "//permissions/principal";

        /// <summary>
        /// The path permission
        /// </summary>
        private const string PathPermission = "//permission";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<PermissionInfo> Parse(XmlNode xml)
        {
            if (xml != null)
            {
                if (xml.NodeListExists(PathPrincipals))
                {
                    return xml.SelectNodes(PathPrincipals)
                           .Cast<XmlNode>()
                           .Select(PermissionInfoParser.Parse)
                           .Where(item => item != null)
                           .ToArray();
                }

                if (xml.NodeExists(PathPermission))
                {
                    return new[] { PermissionInfoParser.Parse(xml.SelectSingleNode(PathPermission)) };
                }
            }

            TraceTool.TraceMessage(string.Format("Node {0} or {1} is empty: no data available", PathPrincipals, PathPermission));

            return Enumerable.Empty<PermissionInfo>();
        }
    }
}
