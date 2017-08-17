using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Haiku
{
    internal static class HaikuLmsUserParser
    {
        public static List<LmsUserDTO> Parse(string xml)
        {
            var xmlDoc = XDocument.Parse(xml);

            var ret = new List<LmsUserDTO>();

            var response = xmlDoc.Root;
            var statusCode = response.Attribute("status").Value;
            if (!"ok".Equals(statusCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Invalid status {statusCode}. Xml={xml}");
            }

            var nodes = response.Descendants("roster");
            if (nodes != null)
            {
                ret.AddRange(from XElement node in nodes select ParseUser(node));
            }

            return ret;
        }

        private static LmsUserDTO ParseUser(XElement node)
        {
            var user = node.Element("user");

            var info = new LmsUserDTO()
            {
                Id = node.Attribute("user_id").Value,
                LmsRole = node.Attribute("role").Value == "S" ? "Student" : "Teacher",
                Login = user.Attribute("login").Value,
                PrimaryEmail = user.Attribute("email")?.Value ?? user.Attribute("unconfirmed_email")?.Value,
                Name = user.Attribute("first_name").Value + " " + user.Attribute("last_name").Value
            };

            return info;
        }
    }
}
