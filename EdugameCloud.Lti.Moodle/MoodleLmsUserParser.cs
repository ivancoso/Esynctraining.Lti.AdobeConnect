using System.Collections.Generic;
using System.Linq;
using System.Xml;
using EdugameCloud.Lti.Extensions;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Moodle
{
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
            info.Id = node.GetNodeValue("id");
            info.Login = node.GetNodeValue("username");
            info.Email = node.GetNodeValue("email");
            info.Name = node.GetNodeValue("fullname");
            info.LmsRole = GetRole(node.GetNodeXmlValues("roles"));
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
                    string name = topRole.GetNodeValue("name");
                    if (!string.IsNullOrWhiteSpace(name))
                        return name;

                    name = topRole.GetNodeValue("shortname");

                    // NOTE: Default names for default roles
                    if (name == "coursecreator")
                        return "Course creator";
                    else if (name == "editingteacher")
                        return "Teacher";
                    else if (name == "teacher")
                        return "Non-editing teacher";
                    else if (name == "user")
                        return "Authenticated user";
                    else if (name == "frontpage")
                        return "Authenticated user on frontpage";

                    return Inflector.Humanize(name);
                }
            }

            return null;
        }

    }

}
