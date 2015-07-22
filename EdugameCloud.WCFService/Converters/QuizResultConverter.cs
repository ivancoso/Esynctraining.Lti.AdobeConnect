namespace EdugameCloud.WCFService.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Core.Utils;

    using Newtonsoft.Json;

    /// <summary>
    /// The quiz result converter.
    /// </summary>
    public abstract class QuizResultConverter
    {
        #region Properties

        /// <summary>
        /// Gets the question model.
        /// </summary>
        protected QuestionModel QuestionModel
        {
            get
            {
                return IoC.Resolve<QuestionModel>();
            }
        }

        #endregion

        public abstract void ConvertAndSendQuizResultToLms(IEnumerable<QuizQuestionResultDTO> results, QuizResult quizResult, LmsUserParameters lmsUserParameters);

        public abstract void ConvertAndSendSurveyResultToLms(IEnumerable<SurveyQuestionResultDTO> results, SurveyResult surveyResult, LmsUserParameters lmsUserParameters);

        /// <summary>
        /// The convert survey answers to quiz answers.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <param name="takeDistractorId">
        /// The take Distractor Id.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        protected string[] ConvertSurveyAnswersToQuizAnswers(Question question, SurveyQuestionResultDTO answer, bool takeDistractorId)
        {
            return answer.answers.Where(a => takeDistractorId ? a.surveyDistractorId != null : a.value != null)
                .Select(a => takeDistractorId ? a.surveyDistractorId.ToString() : a.value).ToArray();
        }

        /// <summary>
        /// The get survey answer lms ids.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        protected string[] GetSurveyAnswerLmsIds(Question question, SurveyQuestionResultDTO answer)
        {
            var answers = answer.answers ?? new SurveyQuestionResultAnswerDTO[] { };

            var distractors = question.Distractors.Where(d => answers.Any(a => a.surveyDistractorAnswerId == d.Id) && d.LmsAnswerId != null);

            return distractors.Select(d => d.LmsAnswerId.ToString()).ToArray();    
        }


        /// <summary>
        /// The get true false lms id answer.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetTrueFalseLmsIdAnswer(Question question, QuizQuestionResultDTO answer)
        {
            Distractor distractor = question.Distractors != null
                ? question.Distractors.FirstOrDefault()
                : null;
            if (distractor != null)
            {
                bool answ = (answer.isCorrect && distractor.IsCorrect.GetValueOrDefault())
                            || (!answer.isCorrect && !distractor.IsCorrect.GetValueOrDefault());
                return answ ? distractor.LmsAnswer : distractor.LmsAnswerId.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// The get true false string answer.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetTrueFalseStringAnswer(Question question, QuizQuestionResultDTO answer)
        {
            Distractor distractor = question.Distractors != null
                ? question.Distractors.FirstOrDefault()
                : null;
            if (distractor != null)
            {
                bool answ = (answer.isCorrect && distractor.IsCorrect.GetValueOrDefault())
                            || (!answer.isCorrect && !distractor.IsCorrect.GetValueOrDefault());

                return string.Format(
                    "{0}{1}",
                    distractor.DistractorName == null || distractor.DistractorName.Equals("truefalse") || distractor.DistractorName.Equals(string.Empty)
                        ? string.Empty
                        : (distractor.DistractorName + "."),
                    answ ? "true" : "false");
            }
            return string.Empty;
        }

        /// <summary>
        /// The is single answer survey.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool IsSingleAnswer(Question question)
        {
            var singleMultChoice = question.SingleMultipleChoiceQuestions.FirstOrDefault();
            return singleMultChoice != null
                   && (singleMultChoice.Restrictions == null || !singleMultChoice.Restrictions.Contains("multi_choice"));
        }

        /// <summary>
        /// The get multiple choice lms ids.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        protected IEnumerable<int> GetMultipleChoiceLmsIds(Question question, QuizQuestionResultDTO answer)
        {
            var answers = answer.answers ?? new string[] { };
            return question.Distractors.Where(
                                q =>
                                answers.Contains(q.Id.ToString(CultureInfo.InvariantCulture)))
                                .Select(d => d.LmsAnswerId.GetValueOrDefault());
        }

        /// <summary>
        /// The get multiple choice lms answers.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        protected IEnumerable<string> GetMultipleChoiceLmsAnswers(Question question, QuizQuestionResultDTO answer)
        {
            var answers = answer.answers ?? new string[] { };
            return question.Distractors.Where(
                                q =>
                                answers.Contains(q.Id.ToString(CultureInfo.InvariantCulture)))
                                .Select(d => d.LmsAnswer);
        }

        /// <summary>
        /// The get multiple choice answers string.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetMultipleChoiceAnswersString(Question question, QuizQuestionResultDTO answer)
        {
            var answers = answer.answers ?? new string[] { };
            var boolValues =
                question.Distractors.Select(q => answers.Contains(q.Id.ToString(CultureInfo.InvariantCulture)) ? "true" : "false");
            return string.Join(";", boolValues);
        }

        /// <summary>
        /// The get dropdown values.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <param name="orderAsKey">
        /// The order as key.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        protected Dictionary<string, string> GetMultipleDropdownValues(Question question, QuizQuestionResultDTO answer, bool orderAsKey)
        {
            var ret = new Dictionary<string, string>();

            var distractor = question.Distractors != null ? question.Distractors.FirstOrDefault() : null;

            if (distractor == null || distractor.LmsAnswer == null)
            {
                return ret;
            }

            var answersDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(distractor.LmsAnswer);
            
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(distractor.DistractorName);
            var textTags = xmlDoc.SelectNodes(string.Format("data//text[@isBlank='true']"));

            var orderKey = 0;

            foreach (var key in answersDict.Keys)
            {
                var order = answersDict[key];
                if (answer.answers.Length > order)
                {
                    var userText = answer.answers[order];

                    var textTag = textTags.Count > order ? textTags[order] : null;
                    if (textTag != null)
                    {
                        var id = textTag.Attributes["id"].Value;
                        var optionTag =
                            xmlDoc.SelectSingleNode(
                                string.Format("data//options[@id='{0}']//option[@name='{1}']", id, userText));
                        if (optionTag != null && optionTag.Attributes["lmsid"] != null)
                        {
                            ret.Add(orderAsKey ? orderAsKey.ToString() : key, optionTag.Attributes["lmsid"].Value);
                            orderKey++;
                        }
                    }
                }
            }

            return ret;
        }

        protected Dictionary<string, string> GetMultipleBlanksValues(Question question, QuizQuestionResultDTO answer)
        {
            var ret = new Dictionary<string, string>();

            var distractor = question.Distractors != null ? question.Distractors.FirstOrDefault() : null;

            if (distractor == null || distractor.LmsAnswer == null)
            {
                return ret;
            }

            var answersDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(distractor.LmsAnswer);
            
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(distractor.DistractorName);

            foreach (var key in answersDict.Keys)
            {
                var order = answersDict[key];
                if (answer.answers.Length > order)
                {
                    var userText = answer.answers[order];

                    ret.Add(key, userText);
                }
            }

            return ret;
        }

        /// <summary>
        /// The get match values.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <param name="useLmsId">
        /// The use lms id.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        protected Dictionary<string, string> GetMatchingValues(
            Question question,
            QuizQuestionResultDTO answer,
            bool useLmsId)
        {            
            var userAnswers = new Dictionary<string, string>();
            if (answer.answers != null)
            {
                answer.answers.ToList().ForEach(
                    a =>
                    {
                        int splitInd = a.IndexOf("$$", System.StringComparison.Ordinal);
                        if (splitInd > -1)
                        {
                            string left = a.Substring(0, splitInd),
                                   right = a.Substring(splitInd + 2, a.Length - splitInd - 2);
                            if (!userAnswers.ContainsKey(left))
                            {
                                userAnswers.Add(left, right);
                            }
                        }
                    });
            }

            if (!useLmsId)
            {
                return userAnswers;
            }

            var ret = new Dictionary<string, string>();

            foreach (var leftDistractor in question.Distractors.OrderBy(d => d.LmsAnswerId))
            {
                var left = leftDistractor.DistractorName.Substring(0, leftDistractor.DistractorName.IndexOf("$$", StringComparison.Ordinal));
                if (userAnswers.ContainsKey(left))
                {
                    var rightDistractor = question.Distractors.FirstOrDefault(d => d.DistractorName.EndsWith("$$" + userAnswers[left]));
                    if (rightDistractor != null)
                    {
                        ret.Add(leftDistractor.LmsAnswerId.ToString(), rightDistractor.LmsAnswer);
                    }
                    else
                    {
                        ret.Add(leftDistractor.LmsAnswerId.ToString(), "0");
                    }
                }
                else
                {
                    ret.Add(leftDistractor.LmsAnswerId.ToString(), "0");
                }
            }

            return ret;
        }
    }
}
