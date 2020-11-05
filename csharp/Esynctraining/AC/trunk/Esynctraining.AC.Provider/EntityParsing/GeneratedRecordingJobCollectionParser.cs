using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class GeneratedRecordingJobCollectionParser
    {
        private const string Path = "//generated-recordings/sco";

        public static IEnumerable<GeneratedRecordingJob> Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<GeneratedRecordingJob>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>()
                .Select(GeneratedRecordingJobParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}
