using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public static class RecordingJobCollectionParser
    {
        private const string Path = "//recording-jobs/recording-job";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<RecordingJob> Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<RecordingJob>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>()
                .Select(RecordingJobParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}
