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
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Utils;

    using Newtonsoft.Json;

    using RestSharp;

    /// <summary>
    /// The quiz result converter.
    /// </summary>
    public sealed class QuizResultConverter
    {
        #region Properties

        /// <summary>
        /// Gets the quiz result model.
        /// </summary>
        private QuizResultModel QuizResultModel
        {
            get
            {
                return IoC.Resolve<QuizResultModel>();
            }
        }

        /// <summary>
        /// Gets the LMS user parameters model.
        /// </summary>
        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        /// <summary>
        /// Gets the survey result model.
        /// </summary>
        private SurveyResultModel SurveyResultModel
        {
            get
            {
                return IoC.Resolve<SurveyResultModel>();
            }
        }

        /// <summary>
        /// Gets the question model.
        /// </summary>
        private QuestionModel QuestionModel
        {
            get
            {
                return IoC.Resolve<QuestionModel>();
            }
        }

        private EGCEnabledMoodleAPI MoodleApi
        {
            get
            {
                return IoC.Resolve<EGCEnabledMoodleAPI>();
            }
        }

        private EGCEnabledBlackboardAPI BlackboardApi
        {
            get
            {
                return IoC.Resolve<EGCEnabledBlackboardAPI>();
            }
        }

        #endregion

        /// <summary>
        /// The convert and send survey result.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        public void ConvertAndSendSurveyResult(IEnumerable<SurveyQuestionResultDTO> results)
        {
            foreach (var userAnswer in results.GroupBy(r => r.surveyResultId))
            {
                var surveyResult = this.SurveyResultModel.GetOneById(userAnswer.Key).Value;
                if (surveyResult == null)
                {
                    continue;
                }

                var lmsUserParameters = surveyResult.LmsUserParametersId.HasValue ? this.LmsUserParametersModel.GetOneById(surveyResult.LmsUserParametersId.Value).Value : null;
                if (lmsUserParameters == null)
                {
                    return;
                }

                var lmsSurveyId = surveyResult.Survey.LmsSurveyId;
                if (lmsSurveyId == null)
                {
                    continue;
                }

                switch (lmsUserParameters.CompanyLms.LmsProvider.Id)
                {
                    case (int)LmsProviderEnum.Moodle:
                        this.ConvertAndSendSurveyResultToMoodle(userAnswer, lmsUserParameters, surveyResult);
                        break;
                    case (int)LmsProviderEnum.Canvas:
                        this.ConvertAndSendSurveyResultToCanvas(userAnswer, lmsUserParameters, lmsSurveyId.Value);
                        break;
                    case (int)LmsProviderEnum.Blackboard:
                        this.ConvertAndSendSurveyResultToBlackboard(userAnswer, lmsUserParameters, lmsSurveyId.Value, surveyResult.Survey.SubModuleItem.Id);
                        break;
                }
            }
        }


        /// <summary>
        /// The convert and send survey result.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <param name="lmsUserParameters">
        /// The lms User Parameters.
        /// </param>
        /// <param name="lmsSurveyId">
        /// The lms Survey Id.
        /// </param>
        public void ConvertAndSendSurveyResultToCanvas(IEnumerable<SurveyQuestionResultDTO> results, LmsUserParameters lmsUserParameters, int lmsSurveyId)
        {
            
            var quizSubmissions = EGCEnabledCanvasAPI.GetSubmissionForQuiz(
                lmsUserParameters.CompanyLms.LmsDomain,
                lmsUserParameters.LmsUser.Token,
                lmsUserParameters.Course,
                lmsSurveyId);

            foreach (var submission in quizSubmissions)
            {
                foreach (var answer in results)
                {
                    Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                    if (question.LmsQuestionId == null)
                    {
                        continue;
                    }

                    QuizQuestionResultDTO quizAnswer = new QuizQuestionResultDTO();
                    quizAnswer.isCorrect = answer.isCorrect;
                    if (question.QuestionType.Id == (int)QuestionTypeEnum.SingleMultipleChoiceText)
                    {
                        quizAnswer.answers =
                            answer.answers.Where(a => a.surveyDistractorAnswerId != null)
                                .Select(a => a.surveyDistractorAnswerId.GetValueOrDefault().ToString())
                                .ToArray();
                    }
                    else
                    {
                        quizAnswer.answers =
                            answer.answers.Where(a => a.value != null)
                                .Select(a => a.value)
                                .ToArray();
                    }
                    var answers = this.ProcessCanvasAnswers(question, quizAnswer);

                    if (answers != null)
                    {
                        submission.quiz_questions.Add(
                            new CanvasQuizSubmissionQuestionDTO { id = question.LmsQuestionId.Value, answer = answers });
                    }
                }

                CanvasAPI.AnswerQuestionsForQuiz(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token,
                    submission);

                EGCEnabledCanvasAPI.ReturnSubmissionForQuiz(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token,
                    lmsUserParameters.Course,
                    submission);
            }
        }

        /// <summary>
        /// The convert and send quiz result.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        public void ConvertAndSendQuizResult(IEnumerable<QuizQuestionResultDTO> results)
        {
            foreach (var userAnswer in results.GroupBy(r => r.quizResultId))
            {
                var quizResult = this.QuizResultModel.GetOneById(userAnswer.Key).Value;
                if (quizResult == null)
                {
                    continue;
                }

                var lmsUserParameters = quizResult.LmsUserParametersId.HasValue ? this.LmsUserParametersModel.GetOneById(quizResult.LmsUserParametersId.Value).Value : null;
                if (lmsUserParameters == null || lmsUserParameters.LmsUser == null)
                {
                    return;
                }

                var lmsQuizId = quizResult.Quiz.LmsQuizId;
                if (lmsQuizId == null)
                {
                    continue;
                }

                switch (lmsUserParameters.CompanyLms.LmsProvider.Id)
                {
                    case (int)LmsProviderEnum.Moodle:
                        this.ConvertAndSendQuizResultToMoodle(userAnswer, lmsUserParameters, quizResult);
                        break;
                    case (int)LmsProviderEnum.Canvas:
                        this.ConvertAndSendQuizResultToCanvas(userAnswer, lmsUserParameters, lmsQuizId.Value);
                        break;
                    case (int)LmsProviderEnum.Blackboard:
                        this.ConvertAndSendQuizResultToBlackboard(userAnswer, lmsUserParameters, lmsQuizId.Value, quizResult.Quiz.SubModuleItem.Id);
                        break;
                }
            }
        }

        /// <summary>
        /// The convert and send result.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <param name="lmsUserParameters">
        /// The lms User Parameters.
        /// </param>
        /// <param name="lmsQuizId">
        /// The lms Quiz Id.
        /// </param>
        public void ConvertAndSendQuizResultToCanvas(IEnumerable<QuizQuestionResultDTO> results, LmsUserParameters lmsUserParameters, int lmsQuizId)
        {
            var quizSubmissions = EGCEnabledCanvasAPI.GetSubmissionForQuiz(
                lmsUserParameters.CompanyLms.LmsDomain, 
                lmsUserParameters.LmsUser.Token, 
                lmsUserParameters.Course, 
                lmsQuizId);

            foreach (var submission in quizSubmissions)
            {
                foreach (var answer in results)
                {
                    Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                    if (question.LmsQuestionId == null)
                    {
                        continue;
                    }

                    var answers = this.ProcessCanvasAnswers(question, answer);

                    if (answers != null)
                    {
                        submission.quiz_questions.Add(
                            new CanvasQuizSubmissionQuestionDTO { id = question.LmsQuestionId.Value, answer = answers });
                    }
                }

                CanvasAPI.AnswerQuestionsForQuiz(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token, 
                    submission);

                EGCEnabledCanvasAPI.ReturnSubmissionForQuiz(
                    lmsUserParameters.CompanyLms.LmsDomain,
                    lmsUserParameters.LmsUser.Token,
                    lmsUserParameters.Course,
                    submission);
            }
        }

        /// <summary>
        /// The convert and send quiz result to blackboard.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <param name="lmsUserParameters">
        /// The lms user parameters.
        /// </param>
        /// <param name="lmsQuizId">
        /// The lms quiz id.
        /// </param>
        public void ConvertAndSendQuizResultToBlackboard(IEnumerable<QuizQuestionResultDTO> results, LmsUserParameters lmsUserParameters, int lmsQuizId, int submoduleItemId)
        {
            Dictionary<int, string> answers = new Dictionary<int, string>();

            foreach (var answer in results)
            {
                Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                if (question.LmsQuestionId == null)
                {
                    continue;
                }

                var userAnswer = this.ProcessBlackboardAnswers(question, answer);

                answers.Add(question.LmsQuestionId.GetValueOrDefault(), userAnswer);
            }

            var questions = this.QuestionModel.GetAllBySubModuleItemId(submoduleItemId);

            var ret = new List<string>();
            foreach (var question in questions)
            {
                ret.Add(answers.ContainsKey(question.LmsQuestionId.GetValueOrDefault()) ? answers[question.LmsQuestionId.GetValueOrDefault()] : "0");
            }

            BlackboardApi.SendAnswers(lmsUserParameters, lmsQuizId.ToString(), false, ret.ToArray());
        }

        /// <summary>
        /// The convert and send survey result to blackboard.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <param name="lmsUserParameters">
        /// The lms user parameters.
        /// </param>
        /// <param name="surveyResult">
        /// The survey result.
        /// </param>
        private void ConvertAndSendSurveyResultToBlackboard(IEnumerable<SurveyQuestionResultDTO> results, LmsUserParameters lmsUserParameters, int lmsSurveyId, int submoduleItemId)
        {
            Dictionary<int, string> answers = new Dictionary<int, string>();

            foreach (var answer in results)
            {
                Question question = this.QuestionModel.GetOneById(answer.questionId).Value;
                if (question.LmsQuestionId == null)
                {
                    continue;
                }

                QuizQuestionResultDTO quizAnswer = new QuizQuestionResultDTO();
                quizAnswer.isCorrect = answer.isCorrect;
                if (question.QuestionType.Id == (int)QuestionTypeEnum.SingleMultipleChoiceText
                    || question.QuestionType.Id == (int)QuestionTypeEnum.RateScaleLikert)
                {
                    quizAnswer.answers =
                        answer.answers.Where(a => a.surveyDistractorAnswerId != null)
                            .Select(a => a.surveyDistractorAnswerId.GetValueOrDefault().ToString())
                            .ToArray();
                }
                else
                {
                    quizAnswer.answers = answer.answers.Where(a => a.value != null).Select(a => a.value).ToArray();
                }
                var userAnswer = this.ProcessBlackboardAnswers(question, quizAnswer);

                answers.Add(question.LmsQuestionId.GetValueOrDefault(), userAnswer);
            }

            var questions = this.QuestionModel.GetAllBySubModuleItemId(submoduleItemId);

            var ret = new List<string>();
            foreach (var question in questions)
            {
                ret.Add(answers.ContainsKey(question.LmsQuestionId.GetValueOrDefault()) ? answers[question.LmsQuestionId.GetValueOrDefault()] : "0");
            }

            BlackboardApi.SendAnswers(lmsUserParameters, lmsSurveyId.ToString(), true, ret.ToArray());
        }

        private string GetTrueFalseLmsIdAnswer(Question question, QuizQuestionResultDTO answer)
        {
            Distractor distractor = question.Distractors != null
                                                    ? question.Distractors.FirstOrDefault()
                                                    : null;
            if (distractor != null)
            {
                var answ = (answer.isCorrect && distractor.IsCorrect.GetValueOrDefault())
                            || (!answer.isCorrect && !distractor.IsCorrect.GetValueOrDefault());
                return answ ? distractor.LmsAnswer : distractor.LmsAnswerId.ToString();
            }

            return string.Empty;
        }

        private string GetTrueFalseStringAnswer(Question question, QuizQuestionResultDTO answer)
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
                    distractor.DistractorName == null || distractor.DistractorName.Equals("truefalse")
                        ? string.Empty
                        : (distractor.DistractorName + "."),
                    answ ? "true" : "false");
            }
            return string.Empty;
        }
        
        /// <summary>
        /// The process blackboard answers.
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
        private string ProcessBlackboardAnswers(Question question, QuizQuestionResultDTO answer)
        {
            switch (question.QuestionType.Id)
            {
                case (int)QuestionTypeEnum.TrueFalse:
                    {
                        return this.GetTrueFalseStringAnswer(question, answer);
                    }
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                case (int)QuestionTypeEnum.RateScaleLikert:
                    {
                        var distractors =
                            question.Distractors.Where(
                                q =>
                                answer.answers != null
                                && answer.answers.Contains(q.Id.ToString(CultureInfo.InvariantCulture)));
                        if (answer.answers != null && answer.answers.Count() == 1)
                        {
                            return
                                question.Distractors.First(
                                    d => answer.answers.Contains(d.Id.ToString(CultureInfo.InvariantCulture)))
                                    .LmsAnswerId.ToString();
                        }
                        else
                        {
                            return string.Join(
                                ";",
                                question.Distractors.Select(
                                    d =>
                                    answer.answers.Contains(d.Id.ToString(CultureInfo.InvariantCulture))
                                        ? "true"
                                        : "false"));
                        }
                    }
                case (int)QuestionTypeEnum.Calculated:
                    {
                        return string.Format("{0};{1}", question.Distractors.First().LmsAnswer, answer.answers.First());
                    }
                case (int)QuestionTypeEnum.Hotspot:
                    {
                        return answer.isCorrect ? question.Distractors.First().LmsAnswer : "0, 0";
                    }
                case (int)QuestionTypeEnum.MultipleDropdowns:
                    {
                        var distractor = question.Distractors != null ? question.Distractors.FirstOrDefault() : null;

                        if (distractor == null || distractor.LmsAnswer == null)
                        {
                            return string.Empty;
                        }

                        var answersDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(distractor.LmsAnswer);

                        List<string> userAnswers = new List<string>();

                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(distractor.DistractorName);
                        var textTags = xmlDoc.SelectNodes(string.Format("data//text[@isBlank='true']"));

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
                                        userAnswers.Add(optionTag.Attributes["lmsid"].Value);
                                    }
                                    else
                                    {
                                        userAnswers.Add("");
                                    }
                                }
                            }
                        }
                        return string.Join(";", userAnswers);
                    }
                case (int)QuestionTypeEnum.Sequence:
                    {
                        var distractors = question.Distractors.OrderBy(d => d.DistractorOrder).ToList();
                        if (answer.isCorrect)
                        {
                            return string.Join(";", distractors.Select(d => d.LmsAnswerId.ToString()));
                        }
                        else
                        {
                            return string.Join(";", distractors.Select(d => (d.DistractorOrder - 1).ToString()));
                        }
                    }
                case (int)QuestionTypeEnum.Matching:
                    {
                        Dictionary<string, string> userAnswers = new Dictionary<string, string>();
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

                        var match = new List<string>();

                        foreach (var distractor in question.Distractors.OrderBy(d => d.LmsAnswerId))
                        {
                            var left = distractor.DistractorName.Substring(
                                0,
                                distractor.DistractorName.IndexOf("$$", System.StringComparison.Ordinal));
                            if (userAnswers.ContainsKey(left))
                            {
                                var answerId =
                                    question.Distractors.FirstOrDefault(
                                        d => d.DistractorName.EndsWith(userAnswers[left]));
                                if (answerId != null)
                                {
                                    match.Add(answerId.LmsAnswerId.ToString());
                                }
                                else
                                {
                                    match.Add("0");
                                }
                            }
                            else
                            {
                                match.Add("0");
                            }
                        }
                        return string.Join(";", match);
                    }
                default:
                    {
                        return answer.answers != null
                                   ? string.Join(";", answer.answers.Select(a => a ?? string.Empty))
                                   : string.Empty;
                    }
            }
        }


        /// <summary>
        /// The convert and send survey result to moodle.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <param name="lmsUserParameters">
        /// The lms user parameters.
        /// </param>
        /// <param name="surveyResult">
        /// The survey result.
        /// </param>
        private void ConvertAndSendSurveyResultToMoodle(IEnumerable<SurveyQuestionResultDTO> results, LmsUserParameters lmsUserParameters, SurveyResult surveyResult)
        {
            var toSend = new List<MoodleQuizResultDTO>();

            foreach (SurveyQuestionResultDTO r in results)
            {
                var m = new MoodleQuizResultDTO();
                
                m.quizId = surveyResult.Survey.LmsSurveyId ?? 0;
                Question question = this.QuestionModel.GetOneById(r.questionId).Value;
                m.questionId = question.LmsQuestionId ?? 0;
                m.questionType = question.QuestionType.Type;
                m.isSingle = question.IsMoodleSingle.GetValueOrDefault();
                m.userId = lmsUserParameters.LmsUser.UserId;
                m.startTime = surveyResult.StartTime.ConvertToUTCTimestamp();

                if (question.QuestionType.Id == (int)QuestionTypeEnum.SingleMultipleChoiceText
                    || question.QuestionType.Id == (int)QuestionTypeEnum.Rate)
                {
                    m.answers = question.Distractors.Where(
                                q =>
                                r.answers != null && r.answers.Any(answ => answ.surveyDistractorAnswerId == q.Id))
                                .Select(q => (q.LmsAnswerId.GetValueOrDefault() + 1).ToString())
                                .ToArray();
                }
                else
                {
                    m.answers = r.answers.Select(a => a.value).ToArray();
                }

                toSend.Add(m);
            }

            if (toSend.Count == 0)
            {
                return;
            }

            var ret =
                toSend.GroupBy(s => s.quizId)
                    .Select(
                        s =>
                        new
                        {
                            surveyId = s.Key,
                            courseId = surveyResult.Survey.SubModuleItem.SubModuleCategory.LmsCourseId ?? 0,
                            usersResults =
                        s.GroupBy(u => new { u.userId, u.startTime })
                        .Select(
                            u =>
                            new
                            {
                                u.Key.userId,
                                u.Key.startTime,
                                answers = u.Select(a => new { a.questionId, answer = a.answers.Aggregate((b, c) => b.ToString() + "|" + c.ToString()) })
                            })
                        });

            string json = (new RestSharp.Serializers.JsonSerializer()).Serialize(ret);
            this.MoodleApi.SendAnswers(lmsUserParameters, json, true, null);
        }

        /// <summary>
        /// The send results to moodle.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <param name="lmsUserParameters">
        /// The lms User Parameters.
        /// </param>
        /// <param name="quizResult">
        /// The quiz Result.
        /// </param>
        private void ConvertAndSendQuizResultToMoodle(IEnumerable<QuizQuestionResultDTO> results, LmsUserParameters lmsUserParameters, QuizResult quizResult)
        {
            var toSend = new List<MoodleQuizResultDTO>();

            foreach (QuizQuestionResultDTO r in results)
            {
                var m = new MoodleQuizResultDTO();
                    

                m.quizId = quizResult.Quiz.LmsQuizId ?? 0;
                Question question = this.QuestionModel.GetOneById(r.questionId).Value;
                m.questionId = question.LmsQuestionId ?? 0;
                m.questionType = question.QuestionType.Type;
                m.isSingle = question.IsMoodleSingle.GetValueOrDefault();
                m.userId = lmsUserParameters.LmsUser.UserId;
                m.startTime = quizResult.StartTime.ConvertToUTCTimestamp();

                switch (question.QuestionType.Id)
                {
                    case (int)QuestionTypeEnum.TrueFalse:
                        m.answers = new [] { this.GetTrueFalseStringAnswer(question, r) };
                        break;
                    case (int)QuestionTypeEnum.CalculatedMultichoice:
                    case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                        m.answers =
                            question.Distractors.Where(
                                q =>
                                r.answers != null && r.answers.Contains(q.Id.ToString(CultureInfo.InvariantCulture)))
                                .Select(q => q.DistractorName)
                                .ToArray();
                        break;
                    case (int)QuestionTypeEnum.Matching:
                        var userAnswers = new Dictionary<string, string>();
                        if (r.answers != null)
                        {
                            r.answers.ToList().ForEach(
                                answer =>
                                    {
                                        int splitInd = answer.IndexOf("$$", System.StringComparison.Ordinal);
                                        if (splitInd > -1)
                                        {
                                            string left = answer.Substring(0, splitInd),
                                                    right = answer.Substring(
                                                        splitInd + 2,
                                                        answer.Length - splitInd - 2);
                                            if (!userAnswers.ContainsKey(left))
                                            {
                                                userAnswers.Add(left, right);
                                            }
                                        }
                                    });
                        }

                        var answers = new List<JsonObject>();
                        foreach (Distractor d in question.Distractors.OrderBy(ds => ds.LmsAnswerId))
                        {
                            string key = d.DistractorName.Substring(
                                0,
                                d.DistractorName.IndexOf("$$", System.StringComparison.Ordinal));
                            if (userAnswers.ContainsKey(key))
                            {
                                var obj = new JsonObject();
                                obj[key] = userAnswers[key];
                                answers.Add(obj);
                            }
                        }

                        m.answers = answers.ToArray();

                        break;
                    default:
                        m.answers = r.answers.ToArray();
                        break;
                }

                toSend.Add(m);
            }

            if (toSend.Count == 0)
            {
                return;
            }

            var ret =
                toSend.GroupBy(s => s.quizId)
                    .Select(
                        s =>
                        new
                            {
                                quizId = s.Key,
                                usersResults =
                            s.GroupBy(u => new { u.userId, u.startTime })
                            .Select(
                                u =>
                                new
                                    {
                                        u.Key.userId,
                                        u.Key.startTime,
                                        answers = u.Select(a => new { a.questionId, a.answers })
                                    })
                            });

            string json = (new RestSharp.Serializers.JsonSerializer()).Serialize(ret);
            this.MoodleApi.SendAnswers(lmsUserParameters, json, false, null);
        }

        /// <summary>
        /// The process answers.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="answer">
        /// The answer.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object ProcessCanvasAnswers(Question question, QuizQuestionResultDTO answer)
        {
            object answers = null;

            Distractor distractor;

            switch (question.QuestionType.Id)
            {
                case (int)QuestionTypeEnum.TextNoQuestion:
                    return null;
                case (int)QuestionTypeEnum.TrueFalse:
                    answers = this.GetTrueFalseLmsIdAnswer(question, answer);
                    break;
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    var multAnswers =
                        question.Distractors.Where(q => answer.answers != null && answer.answers.Contains(q.Id.ToString(CultureInfo.InvariantCulture)))
                            .Select(q => q.LmsAnswerId)
                            .ToList();

                    if (question.IsMoodleSingle.GetValueOrDefault())
                    {
                        answers = multAnswers.FirstOrDefault();
                    }
                    else
                    {
                        answers = multAnswers;
                    }

                    break;
                case (int)QuestionTypeEnum.Matching:
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

                        var match = new List<JsonObject>();

                        foreach (var left in userAnswers.Keys)
                        {
                            var right = userAnswers[left];
                            var distactorLeft =
                                question.Distractors.FirstOrDefault(
                                    d => d.DistractorName.StartsWith(left + "$$"));

                            var distactorRight =
                                question.Distractors.FirstOrDefault(
                                    d => d.DistractorName.EndsWith("$$" + right));
                            if (distactorLeft != null && distactorRight != null)
                            {
                                var jsonObject = new JsonObject();
                                jsonObject.Add("answer_id", distactorLeft.LmsAnswerId);
                                jsonObject.Add("match_id", distactorRight.LmsAnswer);
                                match.Add(jsonObject);
                            }
                        }

                        answers = match;
                        break;
                    }
                case (int)QuestionTypeEnum.MultipleDropdowns:
                case (int)QuestionTypeEnum.FillInTheBlank:
                    {
                        distractor = question.Distractors != null
                                        ? question.Distractors.FirstOrDefault()
                                        : null;

                        if (distractor == null || distractor.LmsAnswer == null)
                        {
                            break;
                        }

                        var answersDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(distractor.LmsAnswer);
                        var obj = new JsonObject();

                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(distractor.DistractorName);
                        var textTags = xmlDoc.SelectNodes(
                            string.Format("data//text[@isBlank='true']"));

                        foreach (var key in answersDict.Keys)
                        {
                            var order = answersDict[key];
                            if (answer.answers.Length > order)
                            {
                                var userText = answer.answers[order];

                                if (question.QuestionType.Id == (int)QuestionTypeEnum.FillInTheBlank)
                                {
                                    obj.Add(key, userText);
                                }
                                else
                                {
                                    int lmsId = 0;
                                    var textTag = textTags.Count > order ? textTags[order] : null;
                                    if (textTag != null)
                                    {
                                        var id = textTag.Attributes["id"].Value;
                                        var optionTag =
                                            xmlDoc.SelectSingleNode(
                                                string.Format("data//options[@id='{0}']//option[@name='{1}']",
                                                id,
                                                userText));
                                        if (optionTag != null && optionTag.Attributes["lmsid"] != null)
                                        {
                                            lmsId = int.Parse(optionTag.Attributes["lmsid"].Value);
                                        }
                                    }

                                    if (lmsId > 0)
                                        obj.Add(key, lmsId);
                                }
                            }
                        }

                        answers = obj;

                        break;
                    }
                default:
                    answers = answer.answers.FirstOrDefault();
                    break;
            }

            return answers;
        }
    }
}
