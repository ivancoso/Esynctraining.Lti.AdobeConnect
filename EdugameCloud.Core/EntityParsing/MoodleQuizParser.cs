namespace EdugameCloud.Core.EntityParsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    using NHibernate.Hql.Ast.ANTLR;
    using NHibernate.Util;

    public class MoodleQuizParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static MoodleQuiz Parse(XmlNode xml)
        {
            if (xml == null)
            {
                return null;
            }

            var single = xml.SelectSingleNode("SINGLE");

            if (single == null)
            {
                return null;
            }

            var ret = new MoodleQuiz();
            
            ret.Id = single.GetNodeValue("id");
            ret.Name = single.GetNodeValue("name");
            ret.Intro = single.GetNodeValue("intro");
            ret.Questions = new List<MoodleQuestion>();

            var questionsNode = single.SelectSingleNode("KEY[@name='questions']");
            if (questionsNode != null)
            {
                var multiple = questionsNode.SelectSingleNode("MULTIPLE");
                if (multiple != null)
                {
                    var questions = multiple.SelectNodes("SINGLE");

                    foreach (XmlNode quest in questions)
                    {
                        var q = new MoodleQuestion();
                        ret.Questions.Add(q);

                        q.Id = quest.GetNodeValue("id");
                        q.Name = quest.GetNodeValue("name");
                        q.QuestionText = quest.GetNodeValue("questiontext");
                        q.QuestionType = quest.GetNodeValue("qtype");

                        var options = quest.SelectSingleNode("KEY[@name='options']");
                        if (options == null)
                        {
                            continue;
                        }
                        var optionsSingle = options.SelectSingleNode("SINGLE");
                        if (optionsSingle == null)
                        {
                            continue;
                        }

                        var answers = optionsSingle.SelectSingleNode("KEY[@name='answers']");
                        if (answers == null)
                        {
                            continue;
                        }
                        
                        var answersMult = answers.SelectSingleNode("MULTIPLE");
                        if (answersMult == null)
                        {
                            continue;
                        }

                        q.Answers = new List<MoodleQuestionOptionAnswer>();
                        var ansrs = answersMult.SelectNodes("SINGLE");

                        foreach (XmlNode answ in ansrs)
                        {
                            var a = new MoodleQuestionOptionAnswer();
                            q.Answers.Add(a);
                            a.Id = answ.GetNodeValue("id");
                            a.Answer = answ.GetNodeValue("answer");
                            a.Fraction = answ.GetNodeValue("fraction");
                        }
                    }
                }

            }
            return ret;
        }
    }
}
