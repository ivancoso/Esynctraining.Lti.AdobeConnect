namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The meeting item collection parser.
    /// </summary>
    internal static class QuizResponseCollectionParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//report-quiz-interactions/row";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static IEnumerable<QuizResponse> Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<QuizResponse>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>()
                .Select(QuizResponseParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }
    }
}
