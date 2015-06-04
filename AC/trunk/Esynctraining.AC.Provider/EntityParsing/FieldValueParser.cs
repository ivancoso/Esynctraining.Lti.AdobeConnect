namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;

    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The field value parser.
    /// </summary>
    internal static class FieldValueParser
    {
        /// <summary>
        /// The path.
        /// </summary>
        private const string Path = "//results//acl-fields//field//value";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Field value string value.</returns>
        public static String Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeListExists(Path))
            {
                return null;
            }

            return xml.SelectSingleNode(Path).SelectSingleNodeValue("text()");
        }
    }
}
