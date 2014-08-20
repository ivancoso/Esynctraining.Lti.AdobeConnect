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
        /// <param name="errorMessage">The error message</param>
        /// <param name="error">The error text</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static MoodleQuiz Parse(XmlNode xml, ref string errorMessage, ref string error)
        {
            if (xml == null)
            {
                return null;
            }

            var exception = xml.SelectSingleNode("EXCEPTION");
            if (exception != null)
            {
                var message = exception.SelectSingleNode("MESSAGE");
                if (message != null)
                {
                    errorMessage = message.InnerText;
                }

                var debugInfo = exception.SelectSingleNode("DEBUGINFO");
                if (debugInfo != null)
                {
                    error = debugInfo.InnerText;
                }    
            }
            

            var response = xml.SelectSingleNode("RESPONSE");

            if (response == null)
            {
                return null;
            }

            var single = response.SelectSingleNode("SINGLE");

            if (single == null)
            {
                return null;
            }

            var ret = new MoodleQuiz();
            
            ret.Id = single.GetNodeValue("id");
            ret.Name = single.GetNodeValue("name");
            ret.Intro = single.GetNodeValue("intro");
            ret.Questions = new List<MoodleQuestion>();

            var courseNode = single.SelectSingleNode("KEY[@name='course']");
            if (courseNode != null)
            {
                var courseSingle = courseNode.SelectSingleNode("SINGLE");
                if (courseSingle != null)
                {
                    int courseIdInt;
                    if (int.TryParse(courseSingle.GetNodeValue("id"), out courseIdInt))
                    {
                        ret.LmsSubmoduleId = courseIdInt;
                    }
                    ret.LmsSubmoduleName = courseSingle.GetNodeValue("fullname");
                }
            }

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
                        q.QuestionType = quest.GetNodeValue("qtype") ?? quest.GetNodeValue("typ");
                        q.Presentation = quest.GetNodeValue("presentation");
                        q.Datasets = new List<MoodleDataset>();

                        var datasetsSingle = quest.SelectNodes("KEY[@name='datasets']/MULTIPLE/SINGLE");
                        if (datasetsSingle != null)
                        {
                            foreach (XmlNode ds in datasetsSingle)
                            {
                                var mds = new MoodleDataset();
                                mds.Id = ds.GetNodeValue("id");
                                mds.Name = ds.GetNodeValue("name");
                                mds.Max = ds.GetNodeValue("maximum"); 
                                mds.Min = ds.GetNodeValue("minimum");

                                q.Datasets.Add(mds);

                                var items = ds.SelectNodes("KEY[@name='items']/MULTIPLE/SINGLE");
                                if (items == null)
                                {
                                    continue;
                                }

                                mds.Items = new List<MoodleDataSetItem>();
                                foreach (XmlNode it in items)
                                {
                                    var mit = new MoodleDataSetItem();
                                    mit.ItemNumber = it.GetNodeValue("itemnumber");
                                    var val = it.GetNodeValue("value");
                                    double value = 0;
                                    if (val != null && double.TryParse(val, out value))
                                    {
                                        mit.Value = value;
                                    }
                                    mds.Items.Add(mit);
                                }

                            }
                        }

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

                        var singleChoice = optionsSingle.GetNodeValue("single");
                        int singleChoiceInt;
                        if (int.TryParse(singleChoice, out singleChoiceInt))
                        {
                            q.IsSingle = singleChoiceInt > 0;
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

                            double tolerance = 0;
                            var tol = answ.GetNodeValue("tolerance");
                            if (tol != null && double.TryParse(tol, out tolerance))
                            {
                                a.Tolerance = tolerance;
                            }
                        }

                        var qz = optionsSingle.SelectSingleNode("KEY[@name='questions']");
                        
                        if (qz == null)
                        {
                            continue;
                        }

                        var qzMult = qz.SelectSingleNode("MULTIPLE");
                        if (qzMult == null)
                        {
                            continue;
                        }

                        q.Questions = new List<MoodleQuestion>();
                        var qzs = qzMult.SelectNodes("SINGLE");

                        if (qzs.Count == 0)
                        {
                            qz = optionsSingle.SelectSingleNode("KEY[@name='subquestions']");
                            if (qz == null)
                            {
                                continue;
                            }

                            qzMult = qz.SelectSingleNode("MULTIPLE");
                            if (qzMult == null)
                            {
                                continue;
                            }

                            qzs = qzMult.SelectNodes("SINGLE");
                        }


                        foreach (XmlNode qzNode in qzs)
                        {
                            var a = new MoodleQuestion();
                            q.Questions.Add(a);
                            a.Id = qzNode.GetNodeValue("id");
                            a.QuestionType = qzNode.GetNodeValue("questiontype");
                            a.QuestionText = qzNode.GetNodeValue("questiontext");
                            a.AnswerText = qzNode.GetNodeValue("answertext");
                        }
                    }
                }

            }
            return ret;
        }
    }
}
