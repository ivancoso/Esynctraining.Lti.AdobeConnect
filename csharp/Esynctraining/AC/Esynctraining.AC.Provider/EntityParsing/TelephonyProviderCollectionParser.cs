namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    internal static class TelephonyProviderCollectionParser
    {
        public static readonly string AccountPath = "//providers-account/provider";
        public static readonly string UserPath = "//providers-user/provider";


        public static IEnumerable<TelephonyProvider> Parse(XmlNode xml, string path)
        {
            if (xml == null || !xml.NodeListExists(path))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", path));

                return Enumerable.Empty<TelephonyProvider>();
            }

            return xml.SelectNodes(path).Cast<XmlNode>()
                .Select(TelephonyProviderParser.Parse)
                .Where(item => item != null)
                .ToArray();
        }

    }

}
