namespace EdugameCloud.Core.EntityParsing
{
    using System.Collections.Generic;
    using System.Xml;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    public class MoodleQuizInfoParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static List<MoodleQuizInfoDTO> Parse(XmlNode xml, ref Dictionary<string, string> courseNamesDictionary )
        {
            var ret = new List<MoodleQuizInfoDTO>();

            if (xml == null)
            {
                return ret;
            }

            var single = xml.SelectSingleNode("SINGLE");

            if (single == null)
            {
                return ret;
            }

            var quizzes = single.SelectSingleNode("KEY[@name='quizzes']") ?? single.SelectSingleNode("KEY[@name='surveys']");

            if (quizzes == null)
            {
                return ret;
            }

            var mult = quizzes.SelectSingleNode("MULTIPLE");

            if (mult == null)
            {
                return ret;
            }

            var infos = mult.SelectNodes("SINGLE");
            foreach (XmlNode i in infos)
            {
                var info = new MoodleQuizInfoDTO();
                info.id = i.GetNodeValue("id");
                info.name = i.GetNodeValue("name");
                info.course = i.GetNodeValue("course");
                info.lastModifiedMoodle = int.Parse(i.GetNodeValue("timemodified"));
                ret.Add(info);
            }

            var courses = single.SelectSingleNode("KEY[@name='courses']");

            if (courses == null)
            {
                return ret;
            }

            mult = courses.SelectSingleNode("MULTIPLE");

            if (mult == null)
            {
                return ret;
            }

            infos = mult.SelectNodes("SINGLE");
            foreach (XmlNode i in infos)
            {
                var id = i.GetNodeValue("id");
                var fullname = i.GetNodeValue("fullname");
                if (!courseNamesDictionary.ContainsKey(id))
                {
                    courseNamesDictionary.Add(id, fullname);
                }
            }

            return ret;
        }
    }
}
