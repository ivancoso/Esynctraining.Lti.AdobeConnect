using System;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    public class QuizQuestionResponseItemParser
    {
        public static QuizQuestionResponseItem Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new QuizQuestionResponseItem
            {
                InteractionId = xml.ParseAttributeLong("interaction-id"),
                PrincipalId = xml.ParseAttributeLong("principal-id"),

                UserName = xml.SelectSingleNodeValue("user-name/text()"),
                Response = xml.SelectSingleNodeValue("response/text()"),
                DateCreated = xml.ParseNodeDateTime("date-created/text()", DateTime.Now),
            };
        }

    }

}
