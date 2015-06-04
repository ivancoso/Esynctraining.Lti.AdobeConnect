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

            // NOTE: account - Information about the account the user belongs to. Returned if you are logged in to Adobe Connect or are making the call on a Adobe Connect hosted account.
            int? accountId = xml.NodeExists("//account") ? int.Parse(xml.SelectSingleNodeValue("//account/@account-id")) : default(int?);

            return new UserInfo
            {
                AccountId = accountId,
                Name = xml.SelectSingleNodeValue("//user/name/text()"),
                Login = xml.SelectSingleNodeValue("//user/login/text()"),
                UserId = xml.SelectSingleNodeValue("//user/@user-id"),
            };
        }
    }
}
