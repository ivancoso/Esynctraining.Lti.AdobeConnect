using System;

namespace EdugameCloud.Core.Extensions
{
    using System.Xml;

    /// <summary>
    /// The XML extensions.
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// The wrap string to XML document.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        public static XmlDocument WrapStringToXmlDocument(this string data)
        {
            var body = new XmlDocument();
            body.LoadXml("<stringContentWrapper></stringContentWrapper>");
            if (!string.IsNullOrWhiteSpace(data))
            {
                if (data.StartsWith("<stringContentWrapper>", StringComparison.OrdinalIgnoreCase))
                {
                    data = data.Replace("<stringContentWrapper>", string.Empty).Replace("</stringContentWrapper>", string.Empty);
                }

                var wrappedData = body.CreateCDataSection(data);
                if (body.DocumentElement != null)
                {
                    body.DocumentElement.AppendChild(wrappedData);
                }
            }

            return body;
        }
    }
}
