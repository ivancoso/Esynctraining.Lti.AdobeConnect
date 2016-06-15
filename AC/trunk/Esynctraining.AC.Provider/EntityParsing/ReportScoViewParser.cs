using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class ReportScoViewCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//results/report-sco-views";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<ReportScoViewContent> Parse(XmlNode xml, string path = null)
        {
            if (xml == null || !xml.NodeListExists(path ?? Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", path ?? Path));

                return Enumerable.Empty<ReportScoViewContent>();
            }

            return xml.SelectNodes(path ?? Path).Cast<XmlNode>()
                .Select(ReportScoViewContentParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}