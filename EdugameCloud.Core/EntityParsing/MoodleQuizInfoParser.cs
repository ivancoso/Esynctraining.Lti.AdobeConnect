namespace EdugameCloud.Core.EntityParsing
{
    using System.Collections.Generic;
    using System.Xml;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Extensions;

    public class MoodleQuizInfoParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static List<MoodleQuizInfoDTO> Parse(XmlNode xml)
        {
            var ret = new List<MoodleQuizInfoDTO>();

            if (xml == null)
            {
                return ret;
            }

            var mult = xml.SelectSingleNode("MULTIPLE");

            if (mult == null)
            {
                return ret;
            }

            var infos = mult.SelectNodes("SINGLE");
            foreach (XmlNode i in infos)
            {
                var info = new MoodleQuizInfoDTO();
                info.Id = i.GetNodeValue("id");
                info.Name = i.GetNodeValue("name");
                ret.Add(info);
            }

            return ret;
        }
    }
}
