using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Esynctraining.Lti.Lms.Common.Dto;

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

        public static List<LmsCourseSectionDTO> ParseSections(string xml)
        {
            var ret = new List<LmsCourseSectionDTO>();
            var xmlDoc = XDocument.Parse(xml);

            var response = xmlDoc.Root;
            var statusCode = response.Attribute("status").Value;
            if (!"ok".Equals(statusCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Invalid status {statusCode}. Xml={xml}");
            }

            var nodes = response.Descendants("roster");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    foreach (var sec in node.Elements("section"))
                    {
                        LmsCourseSectionDTO courseSectionDto = null;
                        string id = sec.Attribute("id").Value;
                        string title = sec.Attribute("title").Value;

                        courseSectionDto = ret.FirstOrDefault(s => s.Id == id);
                        if (courseSectionDto == null)
                        {
                            courseSectionDto = new LmsCourseSectionDTO
                            {
                                Id = id,
                                Name = title,
                                Users = new List<LmsUserDTO>()
                            };
                            ret.Add(courseSectionDto);
                        }
                        courseSectionDto.Users.Add(ParseUser(node));

                    }
                }
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
                Email = user.Attribute("email")?.Value ?? user.Attribute("unconfirmed_email")?.Value,
                Name = user.Attribute("first_name").Value + " " + user.Attribute("last_name").Value,
                SectionIds = node.Elements("section").Select( s => s.Attribute("id").Value).ToList()
            };

            return info;
        }

    }

}
