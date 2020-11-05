namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;
    using Esynctraining.AC.Provider.Utils;

    /// <summary>
    /// The Permission Info parser.
    /// </summary>
    internal static class PermissionInfoParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>PermissionInfo object.</returns>
        public static PermissionInfo Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new PermissionInfo
                {
                    PrincipalId = xml.SelectAttributeValue("principal-id"),
                    PermissionStringValue = xml.SelectAttributeValue("permission-id"),
                    PermissionId = EnumReflector.ReflectEnum(xml.SelectAttributeValue("permission-id"), PermissionId.none),
                    IsPrimary = xml.ParseAttributeBool("is-primary"),
                    HasChildren = xml.ParseAttributeBool("has-children"),
                    Login = xml.SelectSingleNodeValue("login/text()"),
                    Name = xml.SelectSingleNodeValue("name/text()"),
                    Description = xml.SelectSingleNodeValue("description/text()")
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
