﻿namespace EdugameCloud.Core.Extensions
{
    using System.Xml;

    /// <summary>
    /// The moodle parser extension.
    /// </summary>
    public static class MoodleParserExtension
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
    }
}
