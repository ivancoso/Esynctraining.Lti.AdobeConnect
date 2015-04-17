namespace EdugameCloud.Lti.Moodle
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The moodle LMS user parser.
    /// </summary>
    internal static class MoodleLmsUserParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static List<LmsUserDTO> Parse(XmlNode xml)
        {
            var ret = new List<LmsUserDTO>();

            if (xml == null)
            {
                return ret;
            }

            var single = xml.SelectSingleNode("SINGLE");

            if (single != null)
            {
                ret.Add(ParseSingleUser(single));
                return ret;
            }

            var mult = xml.SelectSingleNode("MULTIPLE");

            if (mult == null)
            {
                return ret;
            }

            var nodes = mult.SelectNodes("SINGLE");
            if (nodes != null)
            {
                ret.AddRange(from XmlNode node in nodes select ParseSingleUser(node));
            }

            return ret;
        }

        /// <summary>
        /// The parse single user.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserDTO"/>.
        /// </returns>
        private static LmsUserDTO ParseSingleUser(XmlNode node)
        {
            var info = new LmsUserDTO();
            info.id = node.GetNodeValue("id");
            info.login_id = node.GetNodeValue("username");
            info.primary_email = node.GetNodeValue("email");
            info.name = node.GetNodeValue("fullname");
            info.lms_role = GetRole(node.GetNodeXmlValues("roles"));
            return info;
        }

        /// <summary>
        /// The get role.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetRole(List<XmlNode> roles)
        {
            if (roles != null && roles.Any())
            {
                var topRole = (from r in roles let x = int.Parse(r.GetNodeValue("roleid")) orderby x ascending select r).FirstOrDefault();
                if (topRole != null)
                {
                    return Inflector.Humanize(topRole.GetNodeValue("shortname").Replace("editingteacher", "editing teacher"));
                }
            }

            return null;
        }
    }
}
