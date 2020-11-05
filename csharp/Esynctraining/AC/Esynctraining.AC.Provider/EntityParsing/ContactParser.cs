namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Principal parser.
    /// </summary>
    internal static class ContactParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Principal object.</returns>
        public static Contact Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new Contact
                {
                    FirstName = xml.SelectSingleNodeValue("first-name/text()"),
                    LastName = xml.SelectSingleNodeValue("last-name/text()"),
                    Email = xml.SelectSingleNodeValue("email/text()"),
                };
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
