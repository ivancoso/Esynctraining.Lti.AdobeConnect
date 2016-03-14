using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class SeminarLicensesCollectionParser
    {
        private const string Path = "//sco";

        public static IEnumerable<SeminarLicenseSco> Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<SeminarLicenseSco>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>()
                .Select(SeminarLicenseScoParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}