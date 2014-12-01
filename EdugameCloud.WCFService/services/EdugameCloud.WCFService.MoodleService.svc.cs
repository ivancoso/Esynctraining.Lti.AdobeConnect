// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Script.Serialization;
    using System.Xml;

    using Castle.Windsor.Configuration.AppDomain;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.EntityParsing;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.Moodle;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    using FluentNHibernate.Conventions.AcceptanceCriteria;

    /// <summary>
    ///     The moodle service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MoodleService : BaseService, IMoodleService
    {
        #region Properties

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        private DistractorModel DistractorModel
        {
            get
            {
                return IoC.Resolve<DistractorModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private MoodleUserModel MoodleUserModel
        {
            get
            {
                return IoC.Resolve<MoodleUserModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private QuestionForSingleMultipleChoiceModel QuestionForSingleMultipleChoiceModel
        {
            get
            {
                return IoC.Resolve<QuestionForSingleMultipleChoiceModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private QuestionForTrueFalseModel QuestionForTrueFalseModel
        {
            get
            {
                return IoC.Resolve<QuestionForTrueFalseModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private QuestionModel QuestionModel
        {
            get
            {
                return IoC.Resolve<QuestionModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private QuestionTypeModel QuestionTypeModel
        {
            get
            {
                return IoC.Resolve<QuestionTypeModel>();
            }
        }

        /// <summary>
        ///     Gets the survey grouping type model.
        /// </summary>
        private SurveyGroupingTypeModel SurveyGroupingTypeModel
        {
            get
            {
                return IoC.Resolve<SurveyGroupingTypeModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private QuizFormatModel QuizFormatModel
        {
            get
            {
                return IoC.Resolve<QuizFormatModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private LmsProviderModel LmsProviderModel
        {
            get
            {
                return IoC.Resolve<LmsProviderModel>();
            }
        }

        /// <summary>
        ///     Gets the user parameters model.
        /// </summary>
        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        /// <summary>
        ///     Gets the quiz model.
        /// </summary>
        private QuizModel QuizModel
        {
            get
            {
                return IoC.Resolve<QuizModel>();
            }
        }

        /// <summary>
        ///     Gets the survey model.
        /// </summary>
        private SurveyModel SurveyModel
        {
            get
            {
                return IoC.Resolve<SurveyModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private ScoreTypeModel ScoreTypeModel
        {
            get
            {
                return IoC.Resolve<ScoreTypeModel>();
            }
        }

        /// <summary>
        ///     Gets the company model.
        /// </summary>
        private SubModuleCategoryModel SubModuleCategoryModel
        {
            get
            {
                return IoC.Resolve<SubModuleCategoryModel>();
            }
        }

        /// <summary>
        ///     Gets the submodule category model.
        /// </summary>
        private SubModuleModel SubModuleModel
        {
            get
            {
                return IoC.Resolve<SubModuleModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Converts quiz items into EGC data.
        /// </summary>
        /// <param name="quizesInfo">
        /// The quiz.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizesAndSubModuleItemsDTO> ConvertQuizzes(LmsQuizConvertDTO quizesInfo)
        {
            var serviceResponse = new ServiceResponse<QuizesAndSubModuleItemsDTO>();

            if (quizesInfo.quizIds == null)
            {
                return serviceResponse;
            }

            var user = this.UserModel.GetOneById(quizesInfo.userId).Value;
            var moodleUser = this.MoodleUserModel.GetOneByUserId(quizesInfo.userId);

            if (moodleUser == null)
            {
                serviceResponse.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "TokenNotFound", "No user details were found"));
                return serviceResponse;
            }

            var subModuleItemsQuizes = new Dictionary<int, int>();

            foreach (var id in quizesInfo.quizIds)
            {
                var pairs = new NameValueCollection
                                {
                                    { "wsfunction", "mod_adobeconnect_get_quiz_by_id" }, 
                                    { "wstoken", moodleUser.Token }, 
                                    { "quizId", id.ToString(CultureInfo.InvariantCulture) }
                                };

                byte[] response;
                using (var client = new WebClient())
                {
                    response = client.UploadValues(this.GetServicesUrl(moodleUser.Domain), pairs);
                }

                //var resp = Encoding.UTF8.GetString(response);
                var response2 = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, response);
                var resp = Encoding.Unicode.GetString(response2);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resp);

                string errorMessage = string.Empty, error = string.Empty;
                var q = MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref error);

                if (q == null)
                {
                    serviceResponse.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, errorMessage, error));
                    continue;
                }
                /*
                if (q.Questions != null && q.Questions.Any())
                {
                    var res = this.ConvertAndSave(q, user);
                    if (!subModuleItemsQuizes.ContainsKey(res.Item1))
                    {
                        subModuleItemsQuizes.Add(res.Item1, res.Item2);
                    }
                }
                 * */
            }
            /*
            var items = this.SubModuleItemModel.GetQuizSMItemsByUserId(user.Id).ToList();
            var quizes = this.QuizModel.GetLMSQuizzes(user.Id, 0);

            serviceResponse.@object = new QuizesAndSubModuleItemsDTO
                                          {
                                              quizzes = subModuleItemsQuizes.Select(x => quizes.FirstOrDefault(q => q.quizId == x.Value)).ToList(),
                                              subModuleItems = subModuleItemsQuizes.Select(x => items.FirstOrDefault(q => q.subModuleItemId == x.Key)).ToList(),
                                          };
             */
            return serviceResponse;
        }

        /// <summary>
        /// Converts quiz items into EGC data.
        /// </summary>
        /// <param name="quizesInfo">
        /// The quiz.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveysAndSubModuleItemsDTO> ConvertSurveys(LmsQuizConvertDTO quizesInfo)
        {
            var serviceResponse = new ServiceResponse<SurveysAndSubModuleItemsDTO>();

            if (quizesInfo.quizIds == null)
            {
                return serviceResponse;
            }

            var user = this.UserModel.GetOneById(quizesInfo.userId).Value;
            var moodleUser = this.MoodleUserModel.GetOneByUserId(quizesInfo.userId);

            if (moodleUser == null)
            {
                serviceResponse.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "TokenNotFound", "No user details were found"));
                return serviceResponse;
            }

            var subModuleItemsQuizes = new Dictionary<int, int>();

            foreach (var id in quizesInfo.quizIds)
            {
                var pairs = new NameValueCollection
                                {
                                    { "wsfunction", "mod_adobeconnect_get_survey_by_id" }, 
                                    { "wstoken", moodleUser.Token }, 
                                    { "surveyId", id.ToString(CultureInfo.InvariantCulture) }
                                };

                byte[] response;
                using (var client = new WebClient())
                {
                    response = client.UploadValues(this.GetServicesUrl(moodleUser.Domain), pairs);
                }

                var resp = Encoding.UTF8.GetString(response);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resp);

                string errorMessage = string.Empty, error = string.Empty;
                var q = MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref error);

                if (q == null)
                {
                    serviceResponse.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, errorMessage, error));
                    continue;
                }
                /*
                if (q.Questions != null && q.Questions.Any())
                {
                    try
                    {
                        var res = this.ConvertAndSave(q, user, true);
                        if (!subModuleItemsQuizes.ContainsKey(res.Item1))
                        {
                            subModuleItemsQuizes.Add(res.Item1, res.Item2);
                        }
                    }
                    catch (Exception e)
                    {
                        serviceResponse.SetError(
                            new Error(
                                Errors.CODE_ERRORTYPE_GENERIC_ERROR,
                                "Error during quiz export",
                                "Quiz " + q.Name + " can not be exported: " + e.Message));
                    }
                }
                 * */
            }
            /*
            var items = this.SubModuleItemModel.GetSurveySubModuleItemsByUserId(user.Id).ToList();
            var quizes = this.SurveyModel.GetLmsSurveys(user.Id, 0);

            serviceResponse.@object = new SurveysAndSubModuleItemsDTO()
            {
                surveys = subModuleItemsQuizes.Select(x => quizes.FirstOrDefault(q => q.surveyId == x.Value)).ToList(),
                subModuleItems = subModuleItemsQuizes.Select(x => items.FirstOrDefault(q => q.subModuleItemId == x.Key)).ToList(),
            };
            */
            return serviceResponse;
        }

        public void Test()
        {
            string error;
            var param = LmsUserParametersModel.GetOneById(142).Value;
            var api = IoC.Resolve<LmsFactory>().GetEGCEnabledLmsAPI(LmsProviderEnum.Moodle);
            var i = api.GetItemsForUser(param, false, new int[] { 37 }, out error);
        }

        /// <summary>
        /// Gets all quiz types for user from moodle.
        /// </summary>
        /// <param name="userInfo">
        /// The userInfo.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<MoodleQuizInfoDTO> GetQuizzesForUser(MoodleUserInfoDTO userInfo)
        {
            var serviceResponse = new ServiceResponse<MoodleQuizInfoDTO>();

            var token = userInfo.token ?? this.GetToken(serviceResponse, userInfo.domain, userInfo.name, userInfo.password); 

            if (token == null)
            {
                return serviceResponse;
            }

            var user = this.MoodleUserModel.GetOneByUserIdAndToken(userInfo.userId, token) 
                ?? (userInfo.name != null ? this.MoodleUserModel.GetOneByUserIdAndUserName(userInfo.userId, userInfo.name) : null) 
                ?? this.ConvertDTO(userInfo);

            user.DateModified = DateTime.Now;
            user.Token = token;
            this.MoodleUserModel.RegisterSave(user);

            var pairs = new NameValueCollection
                            {
                                { "wsfunction", "mod_adobeconnect_get_total_quiz_list" }, 
                                { "wstoken", token },
                                { "course", userInfo.courseId }
                            };

            byte[] response;
            using (var client = new WebClient())
            {
                response = client.UploadValues(this.GetServicesUrl(user.Domain), pairs);
            }

            var resp = Encoding.UTF8.GetString(response);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);

            //// var quizes = MoodleQuizInfoParser.Parse(xmlDoc);
            
            return serviceResponse;
        }

        /// <summary>
        /// Gets all surveys for user from moodle.
        /// </summary>
        /// <param name="userInfo">
        /// The userInfo.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<MoodleQuizInfoDTO> GetSurveysForUser(MoodleUserInfoDTO userInfo)
        {
            var serviceResponse = new ServiceResponse<MoodleQuizInfoDTO>();

            var token = userInfo.token ?? this.GetToken(serviceResponse, userInfo.domain, userInfo.name, userInfo.password);

            if (token == null)
            {
                return serviceResponse;
            }

            var user = this.MoodleUserModel.GetOneByUserIdAndToken(userInfo.userId, token)
                ?? (userInfo.name != null ? this.MoodleUserModel.GetOneByUserIdAndUserName(userInfo.userId, userInfo.name) : null)
                ?? this.ConvertDTO(userInfo);

            user.DateModified = DateTime.Now;
            user.Token = token;
            this.MoodleUserModel.RegisterSave(user);

            var pairs = new NameValueCollection
                            {
                                { "wsfunction", "mod_adobeconnect_get_total_survey_list" }, 
                                { "wstoken", token },
                                { "course", userInfo.courseId }
                            };

            byte[] response;
            using (var client = new WebClient())
            {
                response = client.UploadValues(this.GetServicesUrl(user.Domain), pairs);
            }

            var resp = Encoding.UTF8.GetString(response);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);

            var courseNames = new Dictionary<string, string>();
            /*
            var surveys = MoodleQuizInfoParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"), ref courseNames);

            foreach (var survey in surveys)
            {
                var egcSurvey = SurveyModel.GetOneByLmsSurveyId(user.UserId, int.Parse(survey.id), (int)LmsProviderEnum.Moodle).Value;
                if (egcSurvey != null)
                {
                    survey.lastModifiedEGC = egcSurvey.SubModuleItem.DateModified.ConvertToTimestamp();
                }

                survey.courseName = courseNames.ContainsKey(survey.course) ? courseNames[survey.course] : string.Empty;
            }

            serviceResponse.objects = surveys;
            */
            return serviceResponse;
        }

        /// <summary>
        /// The get authentication parameters by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<LmsUserParametersDTO> GetAuthenticationParametersById(string id)
        {
            var result = new ServiceResponse<LmsUserParametersDTO>();

            var param = this.LmsUserParametersModel.GetOneByAcId(id).Value;
            result.@object = param != null ? new LmsUserParametersDTO(param) : null;

            if (param != null)
            {
                this.LmsUserParametersModel.RegisterDelete(param);
            }

            if (result.@object != null)
            {
                result.@object.provider = "Moodle";
                result.@object.domain = "http://64.27.12.60";
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The clear name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ClearName(string name)
        {
            return Regex.Replace(name ?? string.Empty, "<[^>]*(>|$)", string.Empty);
        }

        /// <summary>
        /// The convert and save.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private Tuple<int, int> ConvertAndSave(MoodleQuiz quiz, User user, bool isSurvey, int companyLmsId)
        {
            Tuple<int, int> result;
            var moodleId = string.IsNullOrEmpty(quiz.Id) ? (int?)null : int.Parse(quiz.Id);

            if (!isSurvey)
            {
                var egcQuiz = moodleId.HasValue
                    ? this.QuizModel.GetOneByLmsQuizId(user.Id, moodleId.Value, companyLmsId).Value
                      ?? new Quiz()
                    : new Quiz();

                var submodule = this.ProcessSubModule(user, egcQuiz.IsTransient() ? null : egcQuiz.SubModuleItem, quiz);

                result = this.ProcessQuizData(quiz, egcQuiz, submodule);

                this.ProcessQuizQuestions(quiz, user, submodule);
            }
            else
            {
                var egcSurvey = moodleId.HasValue
                                    ? this.SurveyModel.GetOneByLmsSurveyId(user.Id, moodleId.Value, companyLmsId).Value
                                      ?? new Survey()
                                    : new Survey();

                egcSurvey.SurveyGroupingType = SurveyGroupingTypeModel.GetOneById(2).Value;

                var submodule = this.ProcessSubModule(user, egcSurvey.IsTransient() ? null : egcSurvey.SubModuleItem, quiz);

                result = this.ProcessSurveyData(quiz, egcSurvey, submodule);
                
                this.ProcessSurveyQuestions(quiz, user, submodule);
            }
            return result;
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="MoodleUser"/>.
        /// </returns>
        private MoodleUser ConvertDTO(MoodleUserInfoDTO dto)
        {
            var user = new MoodleUser
                           {
                               Domain = dto.domain, 
                               Password = dto.password, 
                               UserName = dto.name, 
                               UserId = dto.userId
                           };

            return user;
        }

        /// <summary>
        /// The fix domain.
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FixDomain(string domain)
        {
            if (domain.Last() != '/')
            {
                domain = domain + '/';
            }

            if (((string)this.Settings.MoodleChangeUrl).ToLower().Equals("true"))
            {
                return domain.Replace("64.27.12.61", "WIN-J0J791DL0DG").Replace("64.27.12.60", "PRO_Moodle");
            }

            return domain;
        }

        /// <summary>
        /// The get services url.
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetServicesUrl(string domain)
        {
            var serviceUrl = (string)this.Settings.MoodleServiceUrl;
            return this.FixDomain(domain) + (serviceUrl.First() == '/' ? serviceUrl.Substring(1) : serviceUrl);
        }

        /// <summary>
        /// The get token.
        /// </summary>
        /// <param name="serviceResponse">
        /// The service response.
        /// </param>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetToken(ServiceResponse serviceResponse, string domain, string username, string password)
        {
            var pairs = new NameValueCollection
                            {
                                { "username", username }, 
                                { "password", password }, 
                                { "service", "adobe_connect" }
                            };

            byte[] response;
            using (var client = new WebClient())
            {
                response = client.UploadValues(this.GetTokenUrl(domain), pairs);
            }

            var resp = Encoding.UTF8.GetString(response);

            var token = (new JavaScriptSerializer()).Deserialize<MoodleTokenDTO>(resp);

            if (token.error != null)
            {
                serviceResponse.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "TokenNotFound", token.error));
            }

            return token.token;
        }

        /// <summary>
        /// The get token url.
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTokenUrl(string domain)
        {
            var tokenUrl = (string)this.Settings.MoodleTokenUrl;
            return this.FixDomain(domain) + (tokenUrl.First() == '/' ? tokenUrl.Substring(1) : tokenUrl);
        }

        /// <summary>
        /// The process distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="qtype">
        /// The question type.
        /// </param>
        /// <param name="q">
        /// The Moodle question
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        private void ProcessDistractors(User user, QuestionType qtype, MoodleQuestion q, Question question)
        {
            var disctarctorModel = this.DistractorModel;
            switch (qtype.Id)
            {
                case (int)QuestionTypeEnum.FillInTheBlank:
                    {
                        this.ProcessFillInTheBlankDistractors(user, q, question, disctarctorModel);
                        break;
                    }
                case (int)QuestionTypeEnum.ShortAnswer:
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    {
                        this.ProcessSingleMultipleChoiceTextDistractors(user, q, question, disctarctorModel);
                        break;
                    }

                case (int)QuestionTypeEnum.TrueFalse:
                    {
                        this.ProcessQuestionForTrueFalseDistractors(user, q, question, disctarctorModel);
                        break;
                    }

                case (int)QuestionTypeEnum.Numerical:
                    {
                        this.ProcessNumericalDistractors(user, q, question, disctarctorModel);
                        break;
                    }

                case (int)QuestionTypeEnum.Matching:
                    {
                        this.ProcessMatchingDistractors(user, q, question, disctarctorModel);
                        break;
                    }
                case (int)QuestionTypeEnum.Calculated:
                case (int)QuestionTypeEnum.CalculatedMultichoice:
                    {
                        this.ProcessCalculatedDistractors(user, q, question, disctarctorModel);
                        break;
                    }
            }
        }

        /// <summary>
        /// The process fill in the blank question text.
        /// </summary>
        /// <param name="q">
        /// The question.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessFillInTheBlankQuestionText(MoodleQuestion q)
        {
            var i = 1;
            var questionText = ClearName(q.QuestionText);
            foreach (var a in q.Questions)
            {
                if (a.QuestionText == null) continue;
                var index = a.QuestionText.IndexOf(":=");
                if (index < 0) continue;
                var text = a.QuestionText.Substring(index + 2, a.QuestionText.Length - index - 3);
                questionText = questionText.Replace("{#" + (i++) + "}", text);
            }

            return questionText;
        }

        /// <summary>
        /// The process calculated question text.
        /// </summary>
        /// <param name="q">
        /// The question.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessCalculatedQuestionText(MoodleQuestion q)
        {
            var i = 1;
            var questionText = ClearName(q.QuestionText);
            var values = new Dictionary<string, double>();
            foreach (var a in q.Datasets)
            {
                var value = a.Items.Count > 0 ? a.Items.Count : double.Parse(a.Max ?? "0");
                if (!values.ContainsKey(a.Name))
                    values.Add(a.Name, value);
                var name = a.Name;
                questionText = questionText.Replace("{" + name + "}", value.ToString());
            }

            questionText = MoodleExpressionParser.SimplifyExpression(questionText, values);

            return questionText;
        }

        /// <summary>
        /// The process fill in the blank distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="q">
        /// The Moodle question.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        private void ProcessFillInTheBlankDistractors(
            User user, 
            MoodleQuestion q, 
            Question question, 
            DistractorModel distractorModel)
        {
            string blankTrue = "<text id=\"0\" isBlank=\"true\">",
                blankFalse = "<text id=\"0\" isBlank=\"false\">",
                closing = "</text>";
            var distractorText = ClearName(q.QuestionText);
            if (!distractorText.StartsWith("{#"))
            {
                distractorText = blankTrue + distractorText;
            }

            distractorText = distractorText.Replace("{#", closing + blankTrue)
                .Replace("}", closing + blankFalse);
            if (distractorText.StartsWith(closing))
            {
                distractorText = "<data>" + distractorText.Substring(closing.Length);
            }
            else
            {
                distractorText = "<data>" + blankFalse + distractorText;
            }

            var i = 1;
            foreach (var a in q.Questions)
            {
                if (a.QuestionText == null) continue;
                var index = a.QuestionText.IndexOf(":=");
                if (index < 0) continue;
                var text = a.QuestionText.Substring(index + 2, a.QuestionText.Length - index - 3);
                text = blankTrue + text + closing;
                distractorText = distractorText.Replace(blankTrue + (i++) + closing, text);
            }
            if (distractorText.EndsWith(blankFalse))
            {
                distractorText = distractorText.Substring(0, distractorText.Length - blankFalse.Length);
            }
            if (distractorText.EndsWith(blankTrue))
            {
                distractorText = distractorText.Substring(0, distractorText.Length - blankTrue.Length);
            }

            if (!distractorText.EndsWith(closing))
            {
                distractorText = distractorText + closing;
            }

            distractorText = distractorText + "</data>";

            var lmsId = int.Parse(q.Id);

            var distractor = distractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
            distractor.DateModified = DateTime.Now;
            distractor.ModifiedBy = user;
            distractor.DistractorName = distractorText;
            distractor.IsActive = true;
            distractor.DistractorType = null;
            distractor.IsCorrect = true;
                
            distractorModel.RegisterSave(distractor);

        }

        /// <summary>
        /// The process question for true false distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="q">
        /// The moodle question.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        private void ProcessQuestionForTrueFalseDistractors(
            User user, 
            MoodleQuestion q, 
            Question question, 
            DistractorModel distractorModel)
        {
            var lmsId = int.Parse(q.Id);
            var distractor = distractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
            distractor.DateModified = DateTime.Now;
            distractor.ModifiedBy = user;
            distractor.DistractorName = "truefalse";
            distractor.IsActive = true;
            distractor.DistractorType = 1;

            var isCorrect = false;
            foreach (var a in q.Answers)
            {
                var isCorrectAnswer = a.Fraction != null && a.Fraction.StartsWith("1");
                if (a.Answer != null && a.Answer.ToLower().Equals("true") && isCorrectAnswer)
                {
                    isCorrect = true;
                }

            }

            distractor.IsCorrect = isCorrect;
            distractorModel.RegisterSave(distractor);
        }

        /// <summary>
        /// The process matching distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="q">
        /// The moodle question.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        private void ProcessMatchingDistractors(
            User user,
            MoodleQuestion q,
            Question question,
            DistractorModel distractorModel)
        {

            foreach (var a in q.Questions)
            {
                var lmsId = int.Parse(a.Id);
                var name = string.Format("{0}$${1}", ClearName(a.QuestionText), ClearName(a.AnswerText));
                var distractor = distractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = name;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = true;


                distractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The process numerical distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="q">
        /// The moodle question.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        private void ProcessNumericalDistractors(
            User user,
            MoodleQuestion q,
            Question question,
            DistractorModel distractorModel)
        {

            foreach (var a in q.Answers)
            {
                var lmsId = int.Parse(a.Id);
                var name = string.Format("{{\"min\":{0},\"error\":{1}, \"type\":\"Exact\"}}",
                    a.Answer, a.Tolerance);
                var distractor = distractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = name;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = a.Fraction != null && double.Parse(a.Fraction) > 0;

                distractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The process single multiple choice text distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="q">
        /// The moodle question.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        private void ProcessSingleMultipleChoiceTextDistractors(
            User user,
            MoodleQuestion q,
            Question question,
            DistractorModel distractorModel)
        {
            var answerNumber = question.IsMoodleSingle.GetValueOrDefault() ? 0 : 1; // singlechoice starts from 0, multichoice from 1
            foreach (var a in q.Answers)
            {
                var lmsId = int.Parse(a.Id ?? answerNumber.ToString());

                var distractor = distractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = ClearName(a.Answer);
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = a.Fraction != null && double.Parse(a.Fraction) > 0;
                distractor.LmsAnswer = (answerNumber++).ToString();

                distractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The process calculated distractors.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="q">
        /// The moodle question.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        private void ProcessCalculatedDistractors(
            User user,
            MoodleQuestion q,
            Question question,
            DistractorModel distractorModel)
        {
            var answerNumber = question.IsMoodleSingle.GetValueOrDefault() ? 0 : 1; // singlechoice starts from 0, multichoice from 1
            foreach (var a in q.Answers)
            {
                var expression = new MoodleExpressionParser(ClearName(a.Answer));
                foreach (var ds in q.Datasets)
                {
                    var variable = ds.Name;
                    var value = ds.Items.Count > 0 ? ds.Items.First().Value : double.Parse(ds.Max ?? "0");
                    expression.SetValue(variable, value);
                }

                var result = expression.Calculate();

                var name = question.QuestionType.Id == (int)QuestionTypeEnum.CalculatedMultichoice
                    ? result.ToString() 
                    : string.Format("{{\"val\":{0},\"error\":{1} }}",
                    result, a.Tolerance);

                var lmsId = int.Parse(a.Id);
                var distractor = distractorModel.GetOneByQuestionIdAndLmsId(question.Id, lmsId).Value ??
                    new Distractor
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = user,
                        Question = question,
                        LmsAnswerId = lmsId
                    };
                distractor.DateModified = DateTime.Now;
                distractor.ModifiedBy = user;
                distractor.DistractorName = name;
                distractor.IsActive = true;
                distractor.DistractorType = 1;
                distractor.IsCorrect = a.Fraction != null && double.Parse(a.Fraction) > 0;
                distractor.LmsAnswer = (answerNumber++).ToString();

                distractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The process survey data.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="egcSurvey">
        /// The EGC quiz.
        /// </param>
        /// <param name="submoduleItem">
        /// The sub module.
        /// </param>
        private Tuple<int, int> ProcessSurveyData(MoodleQuiz quiz, Survey egcSurvey, SubModuleItem submoduleItem)
        {
            egcSurvey.LmsSurveyId = int.Parse(quiz.Id ?? "0");
            egcSurvey.SurveyName = quiz.Name;
            egcSurvey.SubModuleItem = submoduleItem;
            egcSurvey.Description = Regex.Replace(quiz.Intro, "<[^>]*(>|$)", string.Empty);
            egcSurvey.SubModuleItem.IsShared = true;

            this.SurveyModel.RegisterSave(egcSurvey, true);
            var result = new Tuple<int, int>(submoduleItem.Id, egcSurvey.Id);
            if (!egcSurvey.IsTransient())
            {
                var questionData = this.QuestionModel.GetAllBySubModuleItemId(submoduleItem.Id);
                foreach (var question in questionData)
                {
                    question.IsActive = false;
                    this.QuestionModel.RegisterSave(question);
                    var questionsDistractors = this.DistractorModel.GetAllByQuestionId(question.Id);
                    foreach (var d in questionsDistractors)
                    {
                        d.IsActive = false;
                        this.DistractorModel.RegisterSave(d);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The process quiz data.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="egcQuiz">
        /// The EGC quiz.
        /// </param>
        /// <param name="submoduleItem">
        /// The sub module.
        /// </param>
        private Tuple<int, int> ProcessQuizData(MoodleQuiz quiz, Quiz egcQuiz, SubModuleItem submoduleItem)
        {
            
            egcQuiz.LmsQuizId = int.Parse(quiz.Id ?? "0");
            egcQuiz.QuizName = quiz.Name;
            egcQuiz.SubModuleItem = submoduleItem;
            egcQuiz.Description = Regex.Replace(quiz.Intro, "<[^>]*(>|$)", string.Empty);
            egcQuiz.ScoreType = this.ScoreTypeModel.GetOneById(1).Value;
            egcQuiz.QuizFormat = this.QuizFormatModel.GetOneById(1).Value;
            egcQuiz.SubModuleItem.IsShared = true;

            this.QuizModel.RegisterSave(egcQuiz, true);
            var result = new Tuple<int, int>(submoduleItem.Id, egcQuiz.Id);
            if (!egcQuiz.IsTransient())
            {
                var questionData = this.QuestionModel.GetAllBySubModuleItemId(submoduleItem.Id);
                foreach (var question in questionData)
                {
                    question.IsActive = false;
                    this.QuestionModel.RegisterSave(question);
                    var questionsDistractors = this.DistractorModel.GetAllByQuestionId(question.Id);
                    foreach (var d in questionsDistractors)
                    {
                        d.IsActive = false;
                        this.DistractorModel.RegisterSave(d);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The process quiz questions.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="submodule">
        /// The sub module.
        /// </param>
        private void ProcessQuizQuestions(MoodleQuiz quiz, User user, SubModuleItem submodule)
        {
            var qtypes = this.QuestionTypeModel.GetAllActive().ToList();

            foreach (var quizQuestion in quiz.Questions.Where(qs => qs.QuestionType != null))
            {
                QuestionType questionType = null;/* qtypes.FirstOrDefault(qt => qt.MoodleQuestionType != null 
                    && qt.MoodleQuestionType.Equals(quizQuestion.QuestionType.Equals("calculatedsimple") ? "calculated" : quizQuestion.QuestionType));
                */
                if (questionType == null)
                {
                    continue;
                }

                string questionText;
                switch (questionType.Id)
                {
                    case (int)QuestionTypeEnum.FillInTheBlank:
                        questionText = this.ProcessFillInTheBlankQuestionText(quizQuestion);
                        break;
                    case (int)QuestionTypeEnum.Calculated:
                    case (int)QuestionTypeEnum.CalculatedMultichoice:
                        questionText = this.ProcessCalculatedQuestionText(quizQuestion);
                        break;
                    default:
                        questionText = ClearName(quizQuestion.QuestionText);
                        break;
                }

                var lmsQuestionId = int.Parse(quizQuestion.Id);

                var question = this.QuestionModel.GetOneBySubmoduleItemIdAndLmsId(submodule.Id, lmsQuestionId).Value ??
                        new Question
                                   {
                                       SubModuleItem = submodule,  
                                       DateCreated = DateTime.Now, 
                                       CreatedBy = user, 
                                       LmsQuestionId = lmsQuestionId
                                   };
                question.QuestionName = questionText;
                question.QuestionType = questionType;
                question.DateModified = DateTime.Now;
                question.ModifiedBy = user;
                question.IsActive = true;
                question.IsMoodleSingle = quizQuestion.IsSingle;
                var isTransient = question.Id == 0;

                this.QuestionModel.RegisterSave(question);

                if (isTransient)
                {
                    switch (question.QuestionType.Id)
                    {
                        case (int)QuestionTypeEnum.TrueFalse:
                            this.QuestionForTrueFalseModel.RegisterSave(
                                new QuestionForTrueFalse { Question = question });
                            break;
                        case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                            this.QuestionForSingleMultipleChoiceModel.RegisterSave(
                                new QuestionForSingleMultipleChoice { Question = question });
                            break;
                    }
                }

                this.ProcessDistractors(user, questionType, quizQuestion, question);
            }
        }

        /// <summary>
        /// The process survey questions.
        /// </summary>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="submodule">
        /// The sub module.
        /// </param>
        private void ProcessSurveyQuestions(MoodleQuiz quiz, User user, SubModuleItem submodule)
        {
            var qtypes = this.QuestionTypeModel.GetAllActive().ToList();

            foreach (var quizQuestion in quiz.Questions.Where(qs => qs.QuestionType != null))
            {
                QuestionType questionType = null; /* qtypes.FirstOrDefault(qt => qt.MoodleQuestionType != null
                    && qt.MoodleQuestionType.Equals(quizQuestion.QuestionType.Equals("calculatedsimple") ? "calculated" : quizQuestion.QuestionType));
                */
                if (questionType == null)
                {
                    continue;
                }

                var separatorIndex = quizQuestion.Presentation.IndexOf(">>>>>");
                string questionText = quizQuestion.Name,
                    answers = separatorIndex > 0
                        ? quizQuestion.Presentation.Substring(separatorIndex + 5)
                        : string.Empty,
                    type = separatorIndex > 0 ? quizQuestion.Presentation.Substring(0, separatorIndex) : string.Empty;

                quizQuestion.IsSingle = type.Equals("r");

                quizQuestion.QuestionText = questionText;
                quizQuestion.Answers = answers.Split('|').Select(a => new MoodleQuestionOptionAnswer()
                                                                      {
                                                                          Answer = a
                                                                      }).ToList();
                    

                var lmsQuestionId = int.Parse(quizQuestion.Id);

                var question = this.QuestionModel.GetOneBySubmoduleItemIdAndLmsId(submodule.Id, lmsQuestionId).Value ??
                        new Question
                        {
                            SubModuleItem = submodule,
                            DateCreated = DateTime.Now,
                            CreatedBy = user,
                            LmsQuestionId = lmsQuestionId
                        };
                question.QuestionName = questionText;
                question.QuestionType = questionType;
                question.DateModified = DateTime.Now;
                question.ModifiedBy = user;
                question.IsActive = true;
                question.IsMoodleSingle = quizQuestion.IsSingle;
                var isTransient = question.Id == 0;

                this.QuestionModel.RegisterSave(question);

                if (isTransient)
                {
                    switch (question.QuestionType.Id)
                    {
                        case (int)QuestionTypeEnum.TrueFalse:
                            this.QuestionForTrueFalseModel.RegisterSave(
                                new QuestionForTrueFalse { Question = question });
                            break;
                        case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                            this.QuestionForSingleMultipleChoiceModel.RegisterSave(
                                new QuestionForSingleMultipleChoice { Question = question });
                            break;
                    }
                }

                this.ProcessDistractors(user, questionType, quizQuestion, question);
            }
        }
 

        /// <summary>
        /// The process sub module.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="item">
        /// The submodule item.
        /// </param>
        /// <param name="moodleQuiz">
        /// The Moodle quiz.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItem"/>.
        /// </returns>
        private SubModuleItem ProcessSubModule(User user, SubModuleItem item, MoodleQuiz moodleQuiz)
        {
            var submodule = item ?? new SubModuleItem() { DateCreated = DateTime.Now, CreatedBy = user };

            submodule.IsActive = true;
            submodule.IsShared = true;
            submodule.DateModified = DateTime.Now;
            submodule.ModifiedBy = user;

            var subModuleCategoryModel = this.SubModuleCategoryModel;
            submodule.SubModuleCategory = subModuleCategoryModel.GetOneByLmsCourseIdAndCompanyLms(moodleQuiz.LmsSubmoduleId, (int)LmsProviderEnum.Moodle).Value
                                          ?? new SubModuleCategory
                                                 {
                                                     CategoryName = moodleQuiz.LmsSubmoduleName,
                                                     LmsCourseId = moodleQuiz.LmsSubmoduleId,
                                                     User = user,
                                                     DateModified = DateTime.Now,
                                                     IsActive = true,
                                                     ModifiedBy = user,
                                                     SubModule =
                                                         SubModuleModel.GetOneById(
                                                             (int)SubModuleItemType.Quiz).Value
                                                 };
            if (submodule.SubModuleCategory.IsTransient())
            {
                subModuleCategoryModel.RegisterSave(submodule.SubModuleCategory);
            }

            this.SubModuleItemModel.RegisterSave(submodule);
            return submodule;
        }

        #endregion
    }
}