namespace EdugameCloud.Lti.Moodle
{
    using System.Collections.Generic;
    using System.Xml;

    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;

    /// <summary>
    /// The moodle quiz info parser.
    /// </summary>
    internal sealed class MoodleQuizInfoParser
    {
        /// <summary>
        /// The quizzes path.
        /// </summary>
        private const string QuizzesPath = "RESPONSE/SINGLE/KEY[@name='quizzes']";

        /// <summary>
        /// The surveys path.
        /// </summary>
        private const string SurveysPath = "RESPONSE/SINGLE/KEY[@name='surveys']";

        /// <summary>
        /// The courses path.
        /// </summary>
        private const string CoursesPath = "RESPONSE/SINGLE/KEY[@name='courses']";

        /// <summary>
        /// The information items path.
        /// </summary>
        private const string InfosPath = "MULTIPLE/SINGLE";

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">
        /// The XML.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <returns>
        /// Collection of Meeting Items.
        /// </returns>
        public static List<LmsQuizInfoDTO> Parse(XmlNode xml, bool isSurvey)
        {
            var ret = new List<LmsQuizInfoDTO>();
            XmlNodeList infos;

            var courseNames = new Dictionary<string, string>();

            var courses = xml.SelectSingleNode(CoursesPath);

            if (courses != null)
            {
                infos = courses.SelectNodes(InfosPath);

                if (infos != null)
                {
                    foreach (XmlNode i in infos)
                    {
                        var id = i.GetNodeValue("id");
                        var fullname = i.GetNodeValue("fullname");
                        if (!courseNames.ContainsKey(id))
                        {
                            courseNames.Add(id, fullname);
                        }
                    }
                }
            }

            var quizzes = isSurvey ? xml.SelectSingleNode(SurveysPath) : xml.SelectSingleNode(QuizzesPath);

            if (quizzes == null)
            {
                return ret;
            }

            infos = quizzes.SelectNodes(InfosPath);

            if (infos == null)
            {
                return ret;
            }
            
            foreach (XmlNode i in infos)
            {
                var info = new LmsQuizInfoDTO();
                info.id = int.Parse(i.GetNodeValue("id") ?? "0");
                info.name = i.GetNodeValue("name");
                var course = i.GetNodeValue("course") ?? "0";
                info.course = int.Parse(course);
                info.courseName = courseNames.ContainsKey(course) ? courseNames[course] : string.Empty;
                info.lastModifiedLMS = int.Parse(i.GetNodeValue("timemodified"));
                ret.Add(info);
            }
            
            return ret;
        }
    }
}
