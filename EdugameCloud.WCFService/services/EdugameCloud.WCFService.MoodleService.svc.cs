// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
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

    using FluentValidation.Results;

    using Resources;

    using Weborb.Util;

    /// <summary>
    /// The moodle service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MoodleService : BaseService, IMoodleService
    {
        #region Properties

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
        private SubModuleCategoryModel SubModuleCategoryModel
        {
            get
            {
                return IoC.Resolve<SubModuleCategoryModel>();
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
        private ScoreTypeModel ScoreTypeModel
        {
            get
            {
                return IoC.Resolve<ScoreTypeModel>();
            }
        }

        /// <summary>
        /// Gets the question model.
        /// </summary>
        private DistractorModel DistractorModel
        {
            get
            {
                return IoC.Resolve<DistractorModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        private string GetToken(ServiceResponse serviceResponse, string domain, string username, string password)
        {
            var pairs = new NameValueCollection()
            {
                { "username", username },
                { "password", password },
                { "service", "adobe_connect" }
            };
            
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(this.GetTokenUrl(domain), pairs);
            }
            string resp = System.Text.Encoding.UTF8.GetString(response);

            var token = (new JavaScriptSerializer()).Deserialize<MoodleTokenDTO>(resp);

            if (token.error != null)
            {
                serviceResponse.SetError(new Error(
                        Errors.CODE_ERRORTYPE_GENERIC_ERROR,
                        "TokenNotFound",
                        token.error));

            }

            return token.token;
        }

        /// <summary>
        /// The get quizes.
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

            var user = MoodleUserModel.GetOneByUserIdAndUserName(userInfo.userId, userInfo.name) ?? this.ConvertDTO(userInfo);
            user.DateModified = DateTime.Now;
            user.Token = token;
            MoodleUserModel.RegisterSave(user);

            var pairs = new NameValueCollection()
            {
                { "wsfunction", "mod_adobeconnect_get_total_quiz_list" },
                { "wstoken", token }
            };
            
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(this.GetServicesUrl(user.Domain), pairs);
            }
            string resp = System.Text.Encoding.UTF8.GetString(response);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);

            var quizes = MoodleQuizInfoParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"));
            serviceResponse.objects = quizes;

            return serviceResponse;
        }

        /// <summary>
        /// The convert quizes.
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

            var user = UserModel.GetOneById(quizesInfo.userId).Value;

            var moodleUser = MoodleUserModel.GetOneByUserId(quizesInfo.userId);

            if (moodleUser == null)
            {
                serviceResponse.SetError(new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR,
                                            "TokenNotFound",
                                            "No" 
                                         ));
                return serviceResponse;
            }

            foreach (var id in quizesInfo.quizIds)
            {
                var pairs = new NameValueCollection()
                {
                    { "wsfunction", "mod_adobeconnect_get_quiz_by_id" },
                    { "wstoken", moodleUser.Token },
                    { "quizId", id.ToString() }
                };

                byte[] response = null;
                using (WebClient client = new WebClient())
                {
                    response = client.UploadValues(this.GetServicesUrl(moodleUser.Domain), pairs);
                }
                string resp = System.Text.Encoding.UTF8.GetString(response);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resp);

                var q = MoodleQuizParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"));

                this.ConvertAndSave(q, user);
            }
           

            return serviceResponse;
        }

        private void ConvertAndSave(MoodleQuiz quiz, User user)
        {
            var findQuiz =
                QuizModel.GetAll().FirstOrDefault(q => q.MoodleId.ToString().Equals(quiz.Id ?? "-1"));

            var egcQuiz = findQuiz ?? new Quiz();
            
            var submodule = findQuiz != null ? findQuiz.SubModuleItem : new SubModuleItem();
            submodule.IsActive = true;
            submodule.IsShared = true;
            submodule.DateModified = DateTime.Now;
            submodule.DateCreated = DateTime.Now;
            submodule.CreatedBy = user;
            submodule.SubModuleCategory = SubModuleCategoryModel.GetOneById(466).Value;
            SubModuleItemModel.RegisterSave(submodule);

            egcQuiz.MoodleId = int.Parse(quiz.Id ?? "0");
            egcQuiz.QuizName = quiz.Name;
            egcQuiz.SubModuleItem = submodule;
            egcQuiz.Description = Regex.Replace(quiz.Intro, "<[^>]*(>|$)", "");
            egcQuiz.ScoreType = ScoreTypeModel.GetOneById(1).Value;
            egcQuiz.QuizFormat = QuizFormatModel.GetOneById(1).Value;
             
            QuizModel.RegisterSave(egcQuiz);

            if (findQuiz != null)
            {
                var qData = QuestionModel.GetAllBySubModuleItemId(submodule.Id);
                foreach (var q in qData)
                {
                    q.IsActive = false;
                    QuestionModel.RegisterSave(q);         
                }

                var dData = DistractorModel.GetAllByQuestionId(findQuiz.Id);
                foreach (var d in dData)
                {
                    d.IsActive = false;
                    DistractorModel.RegisterSave(d);
                }

            }

            var qtypes = QuestionTypeModel.GetAllActive();

            foreach (var q in quiz.Questions.Where(qs => qs.QuestionType != null))
            {
                var qtype =
                    qtypes.FirstOrDefault(
                        qt => qt.MoodleQuestionType != null && qt.MoodleQuestionType.Equals(q.QuestionType));
                if (qtype == null) continue;

                var question = new Question();

                question.SubModuleItem = submodule;
                question.QuestionName = Regex.Replace(q.QuestionText, "<[^>]*(>|$)", "");
                question.QuestionType = qtype;
                question.DateModified = DateTime.Now;
                question.DateCreated = DateTime.Now;
                question.CreatedBy = user;
                question.ModifiedBy = user;
                question.IsActive = true;
                question.MoodleQuestionId = int.Parse(q.Id ?? "0");
                
                QuestionModel.RegisterSave(question);

                switch (qtype.Id)
                {
                    case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    {
                        var qm = new QuestionForSingleMultipleChoice();
                        qm.Question = question;

                        QuestionForSingleMultipleChoiceModel.RegisterSave(qm);

                        var isTrue = false;
                        foreach (var a in q.Answers)
                        {
                            var distractor = new Distractor();
                            distractor.DateModified = DateTime.Now;
                            distractor.DateCreated = DateTime.Now;
                            distractor.CreatedBy = user;
                            distractor.ModifiedBy = user;
                            distractor.Question = question;
                            distractor.DistractorName = Regex.Replace(a.Answer, "<[^>]*(>|$)", ""); ;
                            distractor.IsActive = true;
                            distractor.DistractorType = 1;
                            distractor.IsCorrect = a.Fraction != null && double.Parse(a.Fraction) > 0;

                            DistractorModel.RegisterSave(distractor);
                        }
 
                        
                        break;
                    }
                    case (int)QuestionTypeEnum.TrueFalse:
                    {
                        var qtf = new QuestionForTrueFalse();
                        qtf.Question = question;

                        QuestionForTrueFalseModel.RegisterSave(qtf);

                        var distractor = new Distractor();
                        distractor.DateModified = DateTime.Now;
                        distractor.DateCreated = DateTime.Now;
                        distractor.CreatedBy = user;
                        distractor.ModifiedBy = user;
                        distractor.Question = question;
                        distractor.DistractorName = "truefalse";
                        distractor.IsActive = true;
                        distractor.DistractorType = 1;


                        var isTrue = false;
                        foreach (var a in q.Answers)
                        {
                            if (a.Answer != null && a.Answer.ToLower().Equals("true") && a.Fraction != null
                                && a.Fraction.StartsWith("1")) isTrue = true;
                            if (a.Fraction != null && a.Fraction.StartsWith("1"))
                            {
                                distractor.MoodleAnswerId = int.Parse(a.Id ?? "0");
                            }
                            else
                            {
                                distractor.MoodleWrongAnswerId = int.Parse(a.Id ?? "0");
                            }
                        }
                        distractor.IsCorrect = isTrue;

                        DistractorModel.RegisterSave(distractor);
                        break;
                    }
                }

                
            }
            
        }

        #endregion

        #region Methods

        private MoodleUser ConvertDTO(MoodleUserInfoDTO dto)
        {
            var user = new MoodleUser()
                       {
                           Domain = dto.domain,
                           Password = dto.password,
                           UserName = dto.name,
                           UserId = dto.userId
                       };

            return user;
        }

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

        private string GetServicesUrl(string domain)
        {
            var serviceUrl = (string)this.Settings.MoodleServiceUrl;
            return this.FixDomain(domain) + (serviceUrl.First() == '/' ? serviceUrl.Substring(1) : serviceUrl);
        }

        private string GetTokenUrl(string domain)
        {
            var tokenUrl = (string)this.Settings.MoodleTokenUrl;
            return this.FixDomain(domain) + (tokenUrl.First() == '/' ? tokenUrl.Substring(1) : tokenUrl);            
        }

        #endregion
    }
}