namespace Esynctraining.AC.Provider.Extensions
{
    using System;
    using System.Xml;

    using Esynctraining.AC.Provider.Utils;

    /// <summary>
    /// The xml node extensions.
    /// </summary>
    internal static class XmlNodeExtensions
    {
        /// <summary>
        /// The select single node value.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public static string SelectSingleNodeValue(this XmlNode node, string path)
        {
            return node.SelectSingleNode(path)?.Value ?? string.Empty;
        }

        /// <summary>
        /// The parse node integer.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ParseNodeInt(this XmlNode node, string path, int defaultValue = 0)
        {
            return SelectSingleNodeValue(node, path).ParseIntWithDefault(defaultValue);
        }

        /// <summary>
        /// The parse node date time.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ParseNodeDateTime(this XmlNode node, string path, DateTime defaultValue)
        {
            return SelectSingleNodeValue(node, path).ParseDateTimeWithDefault(defaultValue);
        }

        /// <summary>
        /// The parse node date time local.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ParseNodeDateTimeLocal(this XmlNode node, string path, DateTime defaultValue)
        {
            return SelectSingleNodeValue(node, path).ParseDateTimeLocal(defaultValue);
        }

        /// <summary>
        /// The parse node boolean.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ParseNodeBool(this XmlNode node, string path, bool defaultValue = false)
        {
            return node.SelectSingleNodeValue(path).ParseBoolWithDefault(defaultValue);
        }

        /// <summary>
        /// The select attribute value.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string SelectAttributeValue(this XmlNode node, string attribute)
        {
            return node.Attributes != null && node.Attributes[attribute] != null
                ? node.Attributes[attribute].Value
                : null;
        }

        /// <summary>
        /// The parse attribute integer.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ParseAttributeInt(this XmlNode node, string attribute, int defaultValue = 0)
        {
            return SelectAttributeValue(node, attribute).ParseIntWithDefault(defaultValue);
        }

        public static long ParseAttributeLong(this XmlNode node, string attribute, long defaultValue = 0)
        {
            return SelectAttributeValue(node, attribute).ParseLongWithDefault(defaultValue);
        }

        /// <summary>
        /// The parse attribute enumerable.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <typeparam name="T">
        /// Any enumerable.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ParseAttributeEnum<T>(this XmlNode node, string attribute, T defaultValue)
        {
            return EnumReflector.ReflectEnum(node.SelectAttributeValue("type"), defaultValue);
        }

        /// <summary>
        /// The parse attribute boolean.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ParseAttributeBool(this XmlNode node, string path, bool defaultValue = false)
        {
            return node.SelectAttributeValue(path).ParseBoolWithDefault(defaultValue);
        }

        /// <summary>
        /// The node exists.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool NodeExists(this XmlNode node, string path)
        {
            return node.SelectSingleNode(path) != null;
        }

        /// <summary>
        /// The list of nodes exists.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool NodeListExists(this XmlNode node, string path)
        {
            var nodeList = node.SelectNodes(path);

            return nodeList != null && nodeList.Count > 0;
        }

    }

}
