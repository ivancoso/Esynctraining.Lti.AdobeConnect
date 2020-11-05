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
    internal static class ScoContentCollectionParser
    {
        private const string Path = "//scos/sco";


        public static IEnumerable<ScoContent> Parse(XmlNode xml, string path = null)
        {
            if (xml == null || !xml.NodeListExists(path ?? Path))
            {
                TraceTool.TraceMessage($"Node {path ?? Path} is empty: no data available");

                return Enumerable.Empty<ScoContent>();
            }

            return xml.SelectNodes(path ?? Path).Cast<XmlNode>()
                .Select(ScoContentParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }

    }

}
