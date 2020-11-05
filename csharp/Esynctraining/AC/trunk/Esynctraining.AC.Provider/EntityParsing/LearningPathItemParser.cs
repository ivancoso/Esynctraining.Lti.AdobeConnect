namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Learning Path parser.
    /// </summary>
    internal static class LearningPathItemParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>SCO Content.</returns>
        public static LearningPathItem Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new LearningPathItem
                           {
                               CurriculumId = xml.SelectAttributeValue("curriculum-id"),
                               CurrentScoId = xml.SelectAttributeValue("current-sco-id"),
                               TargetScoId = xml.SelectAttributeValue("target-sco-id"),
                               PathType = xml.SelectAttributeValue("path-type"),
                               Name = xml.SelectSingleNodeValue("name/text()"),
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
