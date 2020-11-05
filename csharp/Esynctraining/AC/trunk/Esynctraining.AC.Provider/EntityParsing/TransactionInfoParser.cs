namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Transaction Info parser.
    /// </summary>
    internal static class TransactionInfoParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>SCO Info.</returns>
        public static TransactionInfo Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new TransactionInfo
                       {
                           TransactionId = xml.SelectAttributeValue("transaction-id"),
                           ScoId = xml.SelectAttributeValue("sco-id"),
                           Type = xml.ParseAttributeEnum("type", ScoType.not_set),
                           PrincipalId = xml.SelectAttributeValue("principal-id"),
                           Score = xml.SelectAttributeValue("score"),

                           Name = xml.SelectSingleNodeValue("name/text()"),
                           Url = xml.SelectSingleNodeValue("url/text()"),
                           Login = xml.SelectSingleNodeValue("login/text()"),
                           UserName = xml.SelectSingleNodeValue("user-name/text()"),
                           Status = xml.SelectSingleNodeValue("status/text()"),
                           DateCreated = xml.ParseNodeDateTime("date-created/text()", default(DateTime)),
                           DateClosed = xml.ParseNodeDateTime("date-closed/text()", default(DateTime))
                       };
        }
    }
}
