using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class RecordingCollectionParser
    {
        private const string Path = "//recordings/sco";

        public static IEnumerable<Recording> Parse(XmlNode xml, string path = null)
        {
            if (xml == null || !xml.NodeListExists(path ?? Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", path ?? Path));

                return Enumerable.Empty<Recording>();
            }

            return xml.SelectNodes(path ?? Path).Cast<XmlNode>()
                .Select(RecordingParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}
