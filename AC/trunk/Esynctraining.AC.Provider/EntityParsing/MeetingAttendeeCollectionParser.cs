﻿namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The meeting item collection parser.
    /// </summary>
    internal static class MeetingAttendeeCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//report-meeting-attendance//row";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="serviceUrl">The service URL.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<MeetingAttendee> Parse(XmlNode xml, string serviceUrl)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<MeetingAttendee>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>().Select(MeetingAttendeeParser.Parse).Where(item => item != null).ToArray();
        }
    }
}
