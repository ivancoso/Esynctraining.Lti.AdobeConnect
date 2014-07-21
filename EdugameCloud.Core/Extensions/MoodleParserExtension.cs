namespace EdugameCloud.Core.Extensions
{
    using System.Xml;

    public static class MoodleParserExtension
    {
        public static string GetNodeValue(this XmlNode root, string nodeName)
        {
            var node = root.SelectSingleNode(string.Format("KEY[@name='{0}']", nodeName));
            if (node != null)
            {
                var value = node.SelectSingleNode("VALUE");
                if (value != null)
                    return value.InnerText;
            }
            return null;
        }

    }
}
