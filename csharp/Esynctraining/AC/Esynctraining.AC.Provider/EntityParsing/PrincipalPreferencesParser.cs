namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Principal Preferences parser.
    /// </summary>
    internal static class PrincipalPreferencesParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Principal object.</returns>
        public static PrincipalPreferences Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new PrincipalPreferences
                           {
                               AclId = xml.SelectAttributeValue("acl-id"),
                               Language = xml.SelectAttributeValue("lang"),
                               TimeZoneId = xml.SelectAttributeValue("time-zone-id"),
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
