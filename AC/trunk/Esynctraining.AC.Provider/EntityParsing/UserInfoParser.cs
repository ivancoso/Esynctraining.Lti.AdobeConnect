namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The user info parser.
    /// </summary>
    internal static class UserInfoParser
    {
        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <returns>
        /// The <see cref="UserInfo"/>.
        /// </returns>
        public static UserInfo Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeExists("//user"))
            {
                return null;
            }

            return new UserInfo
                       {
                           Name = xml.SelectSingleNodeValue("//user/name/text()"),
                           Login = xml.SelectSingleNodeValue("//user/login/text()"),
                           UserId = xml.SelectSingleNodeValue("//user/@user-id")
                       };
        }
    }
}
