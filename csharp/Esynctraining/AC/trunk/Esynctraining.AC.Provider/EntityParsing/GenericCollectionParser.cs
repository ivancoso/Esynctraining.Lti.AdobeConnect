using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class GenericCollectionParser<T>
    {
        public static IEnumerable<T> Parse(XmlNode xml, string Path, Func<XmlNode, T> singleElementParser)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", Path));

                return Enumerable.Empty<T>();
            }

            return xml.SelectNodes(Path).Cast<XmlNode>()
                .Select(singleElementParser)
                .Where(item => item != null)
                .ToArray();
        }
    }
}