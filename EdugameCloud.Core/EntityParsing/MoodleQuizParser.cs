namespace EdugameCloud.Core.EntityParsing
{
    using System.Collections.Generic;
    using System.Xml;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    /// <summary>
    /// The moodle quiz parser.
    /// </summary>
    public class MoodleQuizParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="errorMessage">The error message</param>
        /// <param name="error">The error text</param>
        /// <returns>Collection of Meeting Items.</returns>
        public static LmsQuizDTO Parse(XmlNode xml, ref string errorMessage, ref string error)
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

            var ret = new LmsQuizDTO();
            
            ret.id = int.Parse(single.GetNodeValue("id") ?? "0");
            ret.title = single.GetNodeValue("name");
            ret.description = single.GetNodeValue("intro");

            var courseNode = single.SelectSingleNode("KEY[@name='course']");
            if (courseNode != null)
            {
                var courseSingle = courseNode.SelectSingleNode("SINGLE");
                if (courseSingle != null)
                {
                    int courseIdInt;
                    if (int.TryParse(courseSingle.GetNodeValue("id"), out courseIdInt))
                    {
                        ret.course = courseIdInt;
                    }
                    ret.courseName = courseSingle.GetNodeValue("fullname");
                }
            }

            var questionsNode = single.SelectSingleNode("KEY[@name='questions']");
            if (questionsNode != null)
            {
                var multiple = questionsNode.SelectSingleNode("MULTIPLE");
                if (multiple != null)
                {
                    var questions = multiple.SelectNodes("SINGLE");

                    var quizQuestions = new List<QuizQuestionDTO>();
                    foreach (XmlNode quest in questions)
                    {
                        var q = new QuizQuestionDTO();
                        quizQuestions.Add(q);

                        q.id = int.Parse(quest.GetNodeValue("id") ?? "0");
                        q.question_name = quest.GetNodeValue("name");
                        q.question_text = quest.GetNodeValue("questiontext");
                        q.question_type = quest.GetNodeValue("qtype") ?? quest.GetNodeValue("typ");
                        q.presentation = quest.GetNodeValue("presentation");

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

                                q.datasets.Add(mds);

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
                            q.is_single = singleChoiceInt > 0;
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
                        
                        var ansrs = answersMult.SelectNodes("SINGLE");

                        foreach (XmlNode answ in ansrs)
                        {
                            var a = new MoodleQuestionOptionAnswer();
                            q.option_answers.Add(a);
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
                            var a = new AnswerDTO();
                            q.answers.Add(a);
                            a.id = int.Parse(qzNode.GetNodeValue("id") ?? "0");
                            a.question_type = qzNode.GetNodeValue("questiontype");
                            a.question_text = qzNode.GetNodeValue("questiontext");
                            a.text = qzNode.GetNodeValue("answertext");
                        }
                    }

                    ret.questions = quizQuestions.ToArray();
                }

            }
            return ret;
        }
    }
}
