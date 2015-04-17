namespace EdugameCloud.Lti.Moodle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;

    /// <summary>
    ///     The moodle quiz parser.
    /// </summary>
    internal sealed class MoodleQuizParser
    {
        #region Constants

        /// <summary>
        /// The answers path.
        /// </summary>
        private const string AnswersPath = "KEY[@name='answers']/MULTIPLE/SINGLE";

        /// <summary>
        /// The course path.
        /// </summary>
        private const string CoursePath = "KEY[@name='course']/SINGLE";

        /// <summary>
        /// The data set path.
        /// </summary>
        private const string DataSetPath = "KEY[@name='datasets']/MULTIPLE/SINGLE";

        /// <summary>
        /// The debug path.
        /// </summary>
        private const string DebugPath = "EXCEPTION/DEBUGINFO";

        /// <summary>
        /// The message path.
        /// </summary>
        private const string MessagePath = "EXCEPTION/MESSAGE";

        /// <summary>
        /// The option single path.
        /// </summary>
        private const string OptionSinglePath = "KEY[@name='options']/SINGLE";

        /// <summary>
        /// The questions path.
        /// </summary>
        private const string QuestionsPath = "KEY[@name='questions']/MULTIPLE/SINGLE";

        /// <summary>
        /// The quiz path.
        /// </summary>
        private const string QuizPath = "RESPONSE/SINGLE";

        /// <summary>
        /// The sub questions path.
        /// </summary>
        private const string SubquestionsPath = "KEY[@name='subquestions']/MULTIPLE/SINGLE";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">
        /// The XML.
        /// </param>
        /// <param name="errorMessage">
        /// The error message
        /// </param>
        /// <param name="error">
        /// The error text
        /// </param>
        /// <returns>
        /// Collection of Meeting Items.
        /// </returns>
        public static LmsQuizDTO Parse(XmlNode xml, ref string errorMessage, ref string error)
        {
            if (xml == null)
            {
                return null;
            }

            XmlNode message = xml.SelectSingleNode(MessagePath);
            if (message != null)
            {
                errorMessage = message.InnerText;
            }

            XmlNode debugInfo = xml.SelectSingleNode(DebugPath);
            if (debugInfo != null)
            {
                error = debugInfo.InnerText;
            }

            XmlNode quiz = xml.SelectSingleNode(QuizPath);

            if (quiz == null)
            {
                return null;
            }

            var ret = new LmsQuizDTO();

            ret.id = int.Parse(quiz.GetNodeValue("id") ?? "0");
            ret.title = quiz.GetNodeValue("name");
            ret.description = quiz.GetNodeValue("intro");

            XmlNode courseSingle = quiz.SelectSingleNode(CoursePath);
            if (courseSingle != null)
            {
                int courseIdInt;
                if (int.TryParse(courseSingle.GetNodeValue("id"), out courseIdInt))
                {
                    ret.course = courseIdInt;
                }

                ret.courseName = courseSingle.GetNodeValue("fullname");
            }

            XmlNodeList questions = quiz.SelectNodes(QuestionsPath);

            var quizQuestions = new List<LmsQuestionDTO>();
            if (questions != null)
            {
                foreach (XmlNode quest in questions)
                {
                    var q = new LmsQuestionDTO();
                    quizQuestions.Add(q);

                    q.id = int.Parse(quest.GetNodeValue("id") ?? "0");
                    q.question_name = quest.GetNodeValue("name");
                    q.question_text = quest.GetNodeValue("questiontext");
                    q.question_type = quest.GetNodeValue("qtype") ?? quest.GetNodeValue("typ");
                    q.presentation = quest.GetNodeValue("presentation");
                    q.is_mandatory = int.Parse(quest.GetNodeValue("required") ?? "0") > 0;

                    XmlNodeList datasetsSingle = quest.SelectNodes(DataSetPath);
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

                            XmlNodeList items = ds.SelectNodes("KEY[@name='items']/MULTIPLE/SINGLE");
                            if (items == null)
                            {
                                continue;
                            }

                            mds.Items = new List<MoodleDataSetItem>();
                            foreach (XmlNode it in items)
                            {
                                var mit = new MoodleDataSetItem();
                                mit.ItemNumber = it.GetNodeValue("itemnumber");
                                string val = it.GetNodeValue("value");
                                double value;
                                if (val != null && double.TryParse(val, out value))
                                {
                                    mit.Value = value;
                                }

                                mds.Items.Add(mit);
                            }
                        }
                    }

                    XmlNode optionsSingle = quest.SelectSingleNode(OptionSinglePath);
                    if (optionsSingle == null)
                    {
                        continue;
                    }

                    string singleChoice = optionsSingle.GetNodeValue("single");
                    int singleChoiceInt;
                    if (int.TryParse(singleChoice, out singleChoiceInt))
                    {
                        q.is_single = singleChoiceInt > 0;
                    }

                    XmlNodeList answers = optionsSingle.SelectNodes(AnswersPath);

                    if (answers != null)
                    {
                        foreach (XmlNode answ in answers)
                        {
                            var a = new AnswerDTO();
                            q.answers.Add(a);
                            a.id = int.Parse(answ.GetNodeValue("id") ?? "0");
                            a.text = answ.GetNodeValue("answer");
                            a.weight = (int)(double.Parse(answ.GetNodeValue("fraction") ?? "0") * 100);

                            double tolerance;
                            string tol = answ.GetNodeValue("tolerance");
                            if (tol != null && double.TryParse(tol, out tolerance))
                            {
                                a.margin = tolerance;
                            }
                        }
                    }

                    XmlNodeList qzs = optionsSingle.SelectNodes(QuestionsPath);

                    if (qzs != null && qzs.Count == 0)
                    {
                        qzs = optionsSingle.SelectNodes(SubquestionsPath);
                    }

                    if (qzs != null)
                    {
                        foreach (XmlNode quizNode in qzs)
                        {
                            var a = new AnswerDTO();
                            q.answers.Add(a);
                            a.id = int.Parse(quizNode.GetNodeValue("id") ?? "0");
                            a.question_type = quizNode.GetNodeValue("questiontype");
                            a.question_text = quizNode.GetNodeValue("questiontext");
                            a.text = quizNode.GetNodeValue("answertext");
                        }
                    }
                }
            }

            ret.question_list = quizQuestions.ToArray();

            ProcessQuiz(ret);

            return ret;
        }

        private static void ProcessQuiz(LmsQuizDTO quiz)
        {
            foreach (var quizQuestion in quiz.question_list)
            {
                if (quizQuestion.question_text == null)
                {
                    quizQuestion.question_text = quizQuestion.question_name;
                }

                if (quizQuestion.presentation != null)
                {
                    if (quizQuestion.presentation.IndexOf("|", StringComparison.Ordinal) > -1)
                    {
                        var presentationIndex = quizQuestion.presentation.IndexOf("|");
                        quizQuestion.question_text = string.Format(
                            "{0} ({1}-{2})",
                            quizQuestion.question_text,
                            quizQuestion.presentation.Substring(0, presentationIndex),
                            quizQuestion.presentation.Substring(presentationIndex + 1));                        
                    }

                    if (quizQuestion.presentation.IndexOf(">>>>>", StringComparison.Ordinal) > -1)
                    {
                        var separatorIndex = quizQuestion.presentation.IndexOf(">>>>>", System.StringComparison.Ordinal);
                        string answers = separatorIndex > 0
                                             ? quizQuestion.presentation.Substring(separatorIndex + 5)
                                             : quizQuestion.presentation,
                               type = separatorIndex > 0
                                          ? quizQuestion.presentation.Substring(0, separatorIndex)
                                          : string.Empty;
                        if (type.Equals("d"))
                        {
                            quizQuestion.question_type = "dummy_rate";
                        }

                        quizQuestion.is_single = !type.Equals("c");
                        quizQuestion.answers =
                            answers.Split('|')
                                .Select(
                                    (a, i) =>
                                    new AnswerDTO()
                                        {
                                            text =
                                                a.IndexOf("####", StringComparison.Ordinal) > -1
                                                    ? a.Substring(
                                                        a.IndexOf("####", StringComparison.Ordinal)
                                                        + 4)
                                                    : a,
                                            id = i
                                        })
                                .ToList();
                    }
                }
            }
        }

        #endregion
    }
}