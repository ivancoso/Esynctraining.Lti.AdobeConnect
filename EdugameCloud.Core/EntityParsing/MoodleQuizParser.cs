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
        private const string MessagePath = "EXCEPTION/MESSAGE";
        private const string DebugPath = "EXCEPTION/DEBUGINFO";

        private const string QuizPath = "RESPONSE/SINGLE";

        private const string CoursePath = "KEY[@name='course']/SINGLE";

        private const string QuestionsPath = "KEY[@name='questions']/MULTIPLE/SINGLE";

        private const string DataSetPath = "KEY[@name='datasets']/MULTIPLE/SINGLE";

        private const string OptionSinglePath = "KEY[@name='options']/SINGLE";

        private const string AnswersPath = "KEY[@name='answers']/MULTIPLE/SINGLE";

        private const string SubquestionsPath = "KEY[@name='subquestions']/MULTIPLE/SINGLE";


        
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

            var message = xml.SelectSingleNode(MessagePath);
            if (message != null)
            {
                errorMessage = message.InnerText;
            }

            var debugInfo = xml.SelectSingleNode(DebugPath);
            if (debugInfo != null)
            {
                error = debugInfo.InnerText;
            }    

            var quiz = xml.SelectSingleNode(QuizPath);

            if (quiz == null)
            {
                return null;
            }

            var ret = new LmsQuizDTO();
            
            ret.id = int.Parse(quiz.GetNodeValue("id") ?? "0");
            ret.title = quiz.GetNodeValue("name");
            ret.description = quiz.GetNodeValue("intro");
            
            var courseSingle = quiz.SelectSingleNode(CoursePath);
            if (courseSingle != null)
            {
                int courseIdInt;
                if (int.TryParse(courseSingle.GetNodeValue("id"), out courseIdInt))
                {
                    ret.course = courseIdInt;
                }
                ret.courseName = courseSingle.GetNodeValue("fullname");
            }
            
            var questions = quiz.SelectNodes(QuestionsPath);

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
                q.is_mandatory = int.Parse(quest.GetNodeValue("required") ?? "0") > 0;

                var datasetsSingle = quest.SelectNodes(DataSetPath);
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

                var optionsSingle = quest.SelectSingleNode(OptionSinglePath);
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

                var answers = optionsSingle.SelectNodes(AnswersPath);

                foreach (XmlNode answ in answers)
                {
                    var a = new AnswerDTO();
                    q.answers.Add(a);
                    a.id = int.Parse(answ.GetNodeValue("id") ?? "0");
                    a.text = answ.GetNodeValue("answer");
                    a.weight = (int)(double.Parse(answ.GetNodeValue("fraction") ?? "0") * 100);

                    double tolerance = 0;
                    var tol = answ.GetNodeValue("tolerance");
                    if (tol != null && double.TryParse(tol, out tolerance))
                    {
                        a.margin = tolerance;
                    }
                }

                var qzs = optionsSingle.SelectNodes(QuestionsPath);

                if (qzs.Count == 0)
                {
                    qzs = optionsSingle.SelectNodes(SubquestionsPath);
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

            return ret;
        }
    }
}
