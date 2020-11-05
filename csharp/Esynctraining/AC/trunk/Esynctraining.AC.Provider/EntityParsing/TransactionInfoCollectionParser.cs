namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Transaction item collection parser.
    /// </summary>
    internal static class TransactionInfoCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//report-bulk-consolidated-transactions/row";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<TransactionInfo> Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<TransactionInfo>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>()
                .Select(TransactionInfoParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}
