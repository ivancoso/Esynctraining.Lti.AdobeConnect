namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;
    using Esynctraining.AC.Provider.Utils;

    /// <summary>
    /// The User parser.
    /// </summary>
    internal static class UserParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>User or null if it's a folder.</returns>
        public static User Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                var item = new User
                {
                    PrincipalId = xml.SelectAttributeValue("principal-id"),
                    Type = EnumReflector.ReflectEnum(xml.SelectAttributeValue("type"), PrincipalType.user),
                    Name = xml.SelectSingleNodeValue("name/text()"),
                    Login = xml.SelectSingleNodeValue("login/text()"),
                    Email = xml.SelectSingleNodeValue("email/text()"),
                    Manager = xml.SelectSingleNodeValue("manager/text()"),
                };
                return item;
                
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
