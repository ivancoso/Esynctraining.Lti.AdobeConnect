// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
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

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.EntityParsing;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

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
        private QuizModel QuizModel
        {
            get
            {
                return IoC.Resolve<QuizModel>();
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
        public ServiceResponse<int> ConvertQuizes(MoodleQuizConvertDTO quizesInfo)
        {
            var serviceResponse = new ServiceResponse<int>();

            if (quizesInfo.quizIds == null)
            {
                return serviceResponse;
            }

            var user = this.UserModel.GetOneById(quizesInfo.userId).Value;
            var moodleUser = this.MoodleUserModel.GetOneByUserId(quizesInfo.userId);

            if (moodleUser == null)
            {
                serviceResponse.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "TokenNotFound", "No"));
                return serviceResponse;
            }

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

                var resp = Encoding.UTF8.GetString(response);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resp);

                var q = MoodleQuizParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"));

                this.ConvertAndSave(q, user);
            }

            return serviceResponse;
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
        public ServiceResponse<MoodleQuizInfoDTO> GetQuizesForUser(MoodleUserInfoDTO userInfo)
        {
            var serviceResponse = new ServiceResponse<MoodleQuizInfoDTO>();

            var token = this.GetToken(serviceResponse, userInfo.domain, userInfo.name, userInfo.password);

            if (token == null)
            {
                return serviceResponse;
            }

            var user = this.MoodleUserModel.GetOneByUserIdAndUserName(userInfo.userId, userInfo.name) ?? this.ConvertDTO(userInfo);
            user.DateModified = DateTime.Now;
            user.Token = token;
            this.MoodleUserModel.RegisterSave(user);

            var pairs = new NameValueCollection
                            {
                                { "wsfunction", "mod_adobeconnect_get_total_quiz_list" }, 
                                { "wstoken", token }
                            };

            byte[] response;
            using (var client = new WebClient())
            {
                response = client.UploadValues(this.GetServicesUrl(user.Domain), pairs);
            }

            var resp = Encoding.UTF8.GetString(response);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);

            var quizes = MoodleQuizInfoParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"));
            serviceResponse.objects = quizes;

            return serviceResponse;
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
            return Regex.Replace(name, "<[^>]*(>|$)", string.Empty);
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
        private void ConvertAndSave(MoodleQuiz quiz, User user)
        {
            var moodleId = string.IsNullOrEmpty(quiz.Id) ? (int?)null : int.Parse(quiz.Id);
            var egcQuiz = moodleId.HasValue
                               ? this.QuizModel.GetOneByMoodleId(moodleId.Value).Value ?? new Quiz()
                               : new Quiz();

            var submodule = this.ProcessSubModule(user, egcQuiz);

            this.ProcessQuizData(quiz, egcQuiz, submodule);

            this.ProcessQuizQuestions(quiz, user, submodule, moodleId ?? 0);
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
                return domain.Replace("64.27.12.61", "WIN-J0J791DL0DG");
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
            }
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
            foreach (var a in q.Answers)
            {
                var distractor = new Distractor
                                     {
                                         DateModified = DateTime.Now, 
                                         DateCreated = DateTime.Now, 
                                         CreatedBy = user, 
                                         ModifiedBy = user, 
                                         Question = question, 
                                         DistractorName = ClearName(a.Answer), 
                                         IsActive = true, 
                                         DistractorType = null, 
                                         IsCorrect = a.Fraction != null && double.Parse(a.Fraction) > 0
                                     };

                distractorModel.RegisterSave(distractor);
            }
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
            this.QuestionForTrueFalseModel.RegisterSave(new QuestionForTrueFalse { Question = question });

            var distractor = new Distractor
                                 {
                                     DateModified = DateTime.Now, 
                                     DateCreated = DateTime.Now, 
                                     CreatedBy = user, 
                                     ModifiedBy = user, 
                                     Question = question, 
                                     DistractorName = "truefalse", 
                                     IsActive = true, 
                                     DistractorType = 1
                                 };

            var isCorrect = false;
            foreach (var a in q.Answers)
            {
                var isCoorectAnswer = a.Fraction != null && a.Fraction.StartsWith("1");
                if (a.Answer != null && a.Answer.ToLower().Equals("true") && isCoorectAnswer)
                {
                    isCorrect = true;
                }

                if (isCoorectAnswer)
                {
                    distractor.MoodleAnswerId = int.Parse(a.Id ?? "0");
                }
                else
                {
                    distractor.MoodleWrongAnswerId = int.Parse(a.Id ?? "0");
                }
            }

            distractor.IsCorrect = isCorrect;
            distractorModel.RegisterSave(distractor);
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
        /// <param name="submodule">
        /// The sub module.
        /// </param>
        private void ProcessQuizData(MoodleQuiz quiz, Quiz egcQuiz, SubModuleItem submodule)
        {
            egcQuiz.MoodleId = int.Parse(quiz.Id ?? "0");
            egcQuiz.QuizName = quiz.Name;
            egcQuiz.SubModuleItem = submodule;
            egcQuiz.Description = Regex.Replace(quiz.Intro, "<[^>]*(>|$)", string.Empty);
            egcQuiz.ScoreType = this.ScoreTypeModel.GetOneById(1).Value;
            egcQuiz.QuizFormat = this.QuizFormatModel.GetOneById(1).Value;

            this.QuizModel.RegisterSave(egcQuiz);

            if (!egcQuiz.IsTransient())
            {
                var questionData = this.QuestionModel.GetAllBySubModuleItemId(submodule.Id);
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
        /// <param name="moodleId">
        /// The moodle id.
        /// </param>
        private void ProcessQuizQuestions(MoodleQuiz quiz, User user, SubModuleItem submodule, int moodleId)
        {
            var qtypes = this.QuestionTypeModel.GetAllActive().ToList();

            foreach (var quizQuestion in quiz.Questions.Where(qs => qs.QuestionType != null))
            {
                var questionType = qtypes.FirstOrDefault(qt => qt.MoodleQuestionType != null && qt.MoodleQuestionType.Equals(quizQuestion.QuestionType));
                if (questionType == null)
                {
                    continue;
                }

                var question = new Question
                                   {
                                       SubModuleItem = submodule, 
                                       QuestionName = ClearName(quizQuestion.QuestionText), 
                                       QuestionType = questionType, 
                                       DateModified = DateTime.Now, 
                                       DateCreated = DateTime.Now, 
                                       CreatedBy = user, 
                                       ModifiedBy = user, 
                                       IsActive = true, 
                                       MoodleQuestionId = moodleId
                                   };

                this.QuestionModel.RegisterSave(question);
                this.ProcessDistractors(user, questionType, quizQuestion, question);
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
            this.QuestionForSingleMultipleChoiceModel.RegisterSave(
                new QuestionForSingleMultipleChoice { Question = question });

            foreach (var a in q.Answers)
            {
                var distractor = new Distractor
                                     {
                                         DateModified = DateTime.Now, 
                                         DateCreated = DateTime.Now, 
                                         CreatedBy = user, 
                                         ModifiedBy = user, 
                                         Question = question, 
                                         DistractorName = ClearName(a.Answer), 
                                         IsActive = true, 
                                         DistractorType = 1, 
                                         IsCorrect = a.Fraction != null && double.Parse(a.Fraction) > 0
                                     };

                distractorModel.RegisterSave(distractor);
            }
        }

        /// <summary>
        /// The process sub module.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="egcQuiz">
        /// The EGC quiz.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItem"/>.
        /// </returns>
        private SubModuleItem ProcessSubModule(User user, Quiz egcQuiz)
        {
            var submodule = egcQuiz.IsTransient() ? new SubModuleItem() : egcQuiz.SubModuleItem;

            submodule.IsActive = true;
            submodule.IsShared = true;
            submodule.DateModified = DateTime.Now;
            submodule.DateCreated = DateTime.Now;
            submodule.CreatedBy = user;
            var subModuleCategoryModel = this.SubModuleCategoryModel;
            submodule.SubModuleCategory = subModuleCategoryModel.GetOneByNameAndUser(user.Id, "moodle").Value
                                          ?? new SubModuleCategory { CategoryName = "moodle", User = user };
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