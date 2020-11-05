﻿namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The event participant item collection parser.
    /// </summary>
    internal static class EventParticipantsCompleteInformationCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//user_list//user";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<EventParticipantCompleteInformation> Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<EventParticipantCompleteInformation>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>().Select(EventParticipantCompleteInformationParser.Parse).Where(item => item != null).ToArray();
        }
    }
}
