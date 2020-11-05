namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The SCO Info parser.
    /// </summary>
    internal static class QuizResponseParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>SCO Info.</returns>
        public static QuizResponse Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new QuizResponse
            {
                TranscriptId = xml.SelectAttributeValue("transcript-id"),
                ScoId = xml.SelectAttributeValue("sco-id"),
                InteractionId = xml.SelectAttributeValue("interaction-id"),
                DisplaySeq = xml.SelectAttributeValue("display-seq"),
                Score = xml.SelectAttributeValue("score"),
                DateCreated = xml.ParseNodeDateTime("date-created/text()", DateTime.Now),
                Name = xml.SelectSingleNodeValue("name/text()"),
                Description = xml.SelectSingleNodeValue("description/text()"),
                ScoName = xml.SelectSingleNodeValue("sco-name/text()"),
                Response = xml.SelectSingleNodeValue("response/text()"),
            };
        }
    }
}
