namespace EdugameCloud.Lti.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// The moodle parser extension.
    /// </summary>
    public static class MoodleParserExtensions
    {
        /// <summary>
        /// The get node value.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetNodeValue(this XmlNode root, string nodeName)
        {
            var node = root.SelectSingleNode(string.Format("KEY[@name='{0}']", nodeName));
            if (node != null)
            {
                var value = node.SelectSingleNode("VALUE");
                if (value != null)
                {
                    return value.InnerText;
                }
            }

            return null;
        }

        /// <summary>
        /// The get node value.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static List<XmlNode> GetNodeXmlValues(this XmlNode root, string nodeName)
        {
            var result = new List<XmlNode>();
            var node = root.SelectSingleNode(string.Format("KEY[@name='{0}']", nodeName));
            if (node != null)
            {
                var value = node.SelectSingleNode("SINGLE");
                if (value != null)
                {
                    result.Add(value);
                    return result;
                }

                var mult = node.SelectSingleNode("MULTIPLE");

                if (mult != null)
                {
                    var nodes = mult.SelectNodes("SINGLE");
                    if (nodes != null)
                    {
                        result.AddRange(nodes.Cast<XmlNode>());
                    }
                }

                return result;
            }

            return null;
        }
    }
}
