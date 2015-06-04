namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The field collection parser.
    /// </summary>
    internal static class FieldCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//results//acl-fields//field";


        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Fields.</returns>
        public static IEnumerable<Field> Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<Field>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>()
                .Select(FieldParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }

    }

}
