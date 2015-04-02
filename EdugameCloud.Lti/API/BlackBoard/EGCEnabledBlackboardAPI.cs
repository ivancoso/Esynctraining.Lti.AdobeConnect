namespace EdugameCloud.Lti.API.BlackBoard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using BbWsClient;

    using Castle.Core.Logging;

    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Providers;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class EGCEnabledBlackboardAPI : SoapAPI, IEGCEnabledLmsAPI
    {
        public EGCEnabledBlackboardAPI(ApplicationSettingsProvider settings, ILogger logger)
            : base(settings, logger)
        {
        }

        public IEnumerable<LmsQuizInfoDTO> GetItemsInfoForUser(LmsUserParameters lmsUserParameters, bool isSurvey, out string error)
        {
            var quizzes = this.GetItemsForUser(lmsUserParameters, isSurvey, null, out error);
            return quizzes.Select(q => new LmsQuizInfoDTO
            {
                id = q.id,
                name = q.title,
                course = q.course,
                courseName = q.courseName,
                lastModifiedLMS = q.lastModifiedLMS,
                isPublished = q.published
            });
        }

        public IEnumerable<LmsQuizDTO> GetItemsForUser(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds, out string error)
        {
            WebserviceWrapper client = null;

            var tests = this.LoginIfNecessary(
                ref client,
                c =>
                {
                    var egc = c.getEdugameCloudWrapper();
                    /*var content = c.getContentWrapper();

                    var tocs = content.getTOCsByCourseId("_34_1");

                    //var webClient = new WebClient();
                    //webClient.DownloadData(
                    //    "http://blackboard.advantageconnectpro.com/courses/1/egc07/assessment/009f64c3ab3a4327ad2bfcc808930683/night.jpg");

                    var tos = content.getTOCsByCourseId(lmsUserParameters.Course.ToString());

                    if (tos != null)
                    {
                        tos =
                            tos.Where(
                                to =>
                                to.label != null
                                && to.label.Equals("content", StringComparison.InvariantCultureIgnoreCase))
                                .ToArray();
                    }

                    ContentVO[] tsts = null;

                    if (tos != null && tos.Any())
                    {
                        var contentFilter = new ContentFilter()
                        {
                            filterType = 3,
                            filterTypeSpecified = true,
                            contentId = tos.First().courseContentsId
                        };

                        var loaded = content.loadFilteredContent(lmsUserParameters.Course.ToString(), contentFilter);
                        if (loaded != null)
                        {
                            tsts =
                                loaded.Where(
                                    l =>
                                    l.contentHandler != null
                                    && l.contentHandler.Equals(isSurvey ? "resource/x-bb-asmt-survey-link" : "resource/x-bb-asmt-test-link")).ToArray();
                        }
                    }
                    */

                    string testData = string.Empty;
                    /*
                    if (tsts != null)
                    {
                        var quizDTO = new List<LmsQuizDTO>();

                        foreach (var t in tsts)
                        {
                            var lqd = new LmsQuizDTO()
                                    {
                                        course = lmsUserParameters.Course,
                                        courseName = lmsUserParameters.CourseName,
                                        description = t.body,
                                        title = t.title,
                                        id = this.GetBBid(t.id),
                                        published = true
                                    };
                            if (quizIds != null && !quizIds.Contains(lqd.id))
                            {
                                continue;
                            }
                            testData = egc.getAssessmentDetails(t.id, isSurvey);
                            if (testData != null && !testData.StartsWith("Error", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var td = JsonConvert.DeserializeObject<BBAssessmentDTO>(testData);
                                lqd.question_list = td.questions == null
                                                    ? new LmsQuestionDTO[] { }
                                                    : td.questions.Select(
                                                        q =>
                                        new LmsQuestionDTO()
                                            {
                                                question_text = q.text,
                                                question_type = q.type,
                                                question_name = q.title,
                                                id = this.GetBBid(q.id),
                                                answers = this.ParseAnswers(q)
                                            })
                                        .ToArray();
                            }
                            quizDTO.Add(lqd);
                        }

                        return quizDTO.ToList();
                    }
                    */
                    return new List<LmsQuizDTO> { };
                },
                lmsUserParameters.CompanyLms,
                    out error);
            
            return tests;
        }

        private int GetBBid(string id)
        {
            return int.Parse(id.TrimStart("_".ToCharArray()).Split('_').First());
        }

        /// <summary>
        /// The decode formula.
        /// </summary>
        /// <param name="formula">
        /// The formula.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DecodeFormula(string formula)
        {
            if (formula == null)
            {
                return null;
            }
            formula = HttpUtility.HtmlDecode(formula);
            formula = formula.Replace("<mi>", "[").Replace("</mi>", "]");
            return formula;
        }

        private List<AnswerDTO> ParseAnswers(BBQuestionDTO q)
        {
            var correctAnswerId = 0;

            if (!string.IsNullOrEmpty(q.answer))
            {
                if (q.answers == null && q.answersList == null)
                {
                    string answerFirstPart = null;
                    if (!string.IsNullOrEmpty(q.answersChoices))
                    {
                        var dotIndex = q.answersChoices.IndexOf(".");
                        answerFirstPart = q.answersChoices.Substring(0, dotIndex);
                    }

                    double margin = 0;
                    if (q.answerRange != null)
                    {
                        double.TryParse(q.answerRange, out margin);
                    }
                    return new List<AnswerDTO>() { new AnswerDTO() { text = q.answer, weight = 100, id = 0, margin = margin, question_text = answerFirstPart } };
                }
                else
                {
                    correctAnswerId = int.Parse(q.answer);
                }
            }

            var ret = new List<AnswerDTO>();

            if (q.answersList is JObject)
            {
                var answersList = q.answersList as JObject;

                if (answersList["image"] != null)
                {
                    var coords = answersList["coord"].ToString();//.Split(',').Select(z => int.Parse(z.Trim())).ToArray();
                    if (coords.Length < 4)
                    {
                        return ret;
                    }
                    var image = answersList["image"].ToString();

                    ret.Add(new AnswerDTO()
                                {
                                    text = coords,
                                    question_text = image
                                });
                    return ret;
                }

                var i = 0;
                foreach (var answer in answersList)
                {
                    bool isList = false;
                    var stringValue = string.Empty;
                    if (answer.Value is JContainer && answer.Value.Any())
                    {
                        isList = true;
                        stringValue = answer.Value.First().ToString();
                    }
                    else if (answer.Value != null)
                    {
                        stringValue = answer.Value.ToString();
                    }
                    
                    ret.Add(new AnswerDTO()
                    {
                        id = i++,
                        text = isList ? stringValue : answer.Key,
                        blank_id = answer.Key,
                        weight = stringValue.Equals("true", StringComparison.InvariantCultureIgnoreCase) ? 100 : 0
                    });
                }
                return ret;
            }

            if (q.variableSets is JObject)
            {
                var variableSets = q.variableSets as JObject;
                foreach (var set in variableSets)
                {
                    double tolerance = 0;
                    double.TryParse(q.tolerance, out tolerance);
                    
                    var quizAnswer = new AnswerDTO()
                    {
                        id = 0,
                        margin = tolerance,
                        question_text = set.Key,
                        text = this.DecodeFormula(q.formula),
                        weight = 100
                    };

                    if (set.Value is JObject)
                    {
                        foreach (var variable in set.Value as JObject)
                        {
                            if (variable.Key.Equals("answer"))
                            {
                                quizAnswer.answer = double.Parse(variable.Value.ToString());
                            }
                            else
                            {
                                quizAnswer.variables.Add(new VariableDTO()
                                {
                                    name = variable.Key,
                                    value = variable.Value.ToString()
                                });                                
                            }
                        }
                    }

                    ret.Add(quizAnswer);

                    break;
                }
                return ret;
            }

            if (q.answers is JObject)
            {
                var answersList = q.answers as JObject;
                var i = 0;
                if (answersList.Count > 0)
                {
                    foreach (var answer in answersList)
                    {
                        var rightAnswer = answer.Value != null ? answer.Value.ToString() : string.Empty;

                        if (q.choices is JObject)
                        {
                            var currentChoice = (q.choices as JObject)[answer.Key];

                            if (currentChoice != null)
                            {
                                foreach (var option in currentChoice as JObject)
                                {
                                    var answerDto = new AnswerDTO()
                                    {
                                        id = i++,
                                        blank_id = answer.Key,
                                        match_id = option.Key,
                                        text = option.Value.ToString(),
                                        weight = option.Key.Equals(rightAnswer) ? 100 : 0
                                    };

                                    ret.Add(answerDto);
                                }
                            }
                        }
                    }                    
                }
                else if (q.choices is JObject)
                {
                    foreach (var choice in q.choices as JObject)
                    {
                        foreach (var option in choice.Value as JObject)
                        {
                            var answerDto = new AnswerDTO()
                            {
                                id = i++,
                                blank_id = choice.Key,
                                match_id = option.Key,
                                text = option.Value.ToString()
                            };

                            ret.Add(answerDto);                            
                        }
                    }
                }
                return ret;
            }

            if (q.answerPhrasesList != null || q.questionWordsList != null)
            {
                var i = 0;
                foreach (var question in q.questionWordsList ?? new string[] { string.Empty })
                {
                    foreach (var phrase in q.answerPhrasesList ?? new string[] { string.Empty })
                    {
                        var answer = string.Format("{0} {1}", question, phrase).Trim();
                        ret.Add(new AnswerDTO() { text = answer, weight = 100, id = i++ });
                    }
                }

                return ret;
            }

            if (q.answersList is JContainer)
            {
                List<string> answers = null;
                if (q.answers is JContainer)
                {
                    answers = (q.answers as JContainer).Select(t => t.ToString()).ToList();
                }
                var answersList = q.answersList as JContainer;
                var i = 0;
                foreach (var answer in answersList)
                {
                    int order = 0;
                    string questionText = null, answerText = null;

                    if (answer is JObject)
                    {
                        foreach (var option in answer as JObject)
                        {
                            questionText = option.Key;
                            answerText = option.Value.ToString();
                            break;
                        }
                    }
                    else
                    {
                        answerText = answer.ToString();
                        int.TryParse(answerText, out order);
                    }
                    
                    ret.Add(new AnswerDTO()
                    {
                        id = i,
                        text = answers != null && answers.Count > i ? answers[i] : answerText,
                        order = order,
                        question_text = questionText,
                        weight = i == correctAnswerId ? 100 : 0
                    });
                    i++;
                }
                return ret;
            }

            if (q.answersList is JObject)
            {
                var answer = q.answersList as JObject;
                string[] corrdinates = answer["coord"].ToString().Split(',');

            }


            return new List<AnswerDTO>() { new AnswerDTO() { text = "no answer", weight = 100, id = 0 } };
        }

        /// <summary>
        /// The send answers.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The lms user parameters.
        /// </param>
        /// <param name="contentId">
        /// The contenId.
        /// </param>
        /// <param name="isSurvey">
        /// The is survey.
        /// </param>
        /// <param name="answers">
        /// The answers.
        /// </param>
        public void SendAnswers(LmsUserParameters lmsUserParameters, string contentId, bool isSurvey, string[] answers)
        {
            WebserviceWrapper client = null;

            string contentId1 = contentId;
            var tests = this.LoginIfNecessary(
                ref client,
                c =>
                    {
                        var egc = c.getEdugameCloudWrapper();
                        string attempt = null;
                        
                        /*
                        if (isSurvey)
                        {
                            attempt = egc.initializeExternalTestResult(contentId1, lmsUserParameters.Wstoken);
                            if (attempt != null && !attempt.StartsWith("_"))
                            {
                                attempt = null;
                            }
                        }
                        */

                        return egc.submitTestResult(
                            contentId1,
                            lmsUserParameters.Wstoken,
                            answers,
                            attempt,
                            isSurvey);
                    },
                lmsUserParameters.CompanyLms,
                out contentId);
        }
    }
}
