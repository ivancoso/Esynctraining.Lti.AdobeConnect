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

        private string GetToken(string domain, string username, string password)
        {
            string uri = domain + Settings.MoodleTokenUrl;
            var pairs = new NameValueCollection()
            {
                { "username", username },
                { "password", password },
                { "service", "adobe_connect" }
            };
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, pairs);
            }
            string resp = System.Text.Encoding.UTF8.GetString(response);

            var token = (new JavaScriptSerializer()).Deserialize<MoodleUserDTO>(resp).token;

            return token;
        }

        /// <summary>
        /// The get quizes.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="username">
        /// The token.
        /// </param>
        /// <param name="password">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<MoodleQuizInfoDTO> GetQuizesForUser(MoodleUserInfoDTO userInfo)
        {
            while (userInfo.domain.Last() == '/')
            {
                userInfo.domain = userInfo.domain.Remove(userInfo.domain.Length - 1);
            }
            if (((string)this.Settings.MoodleChangeUrl).ToLower().Equals("true"))
            {
                userInfo.domain = userInfo.domain.Replace("64.27.12.61", "WIN-J0J791DL0DG");
            }

            var token = this.GetToken(userInfo.domain, userInfo.name, userInfo.password);

            var user = MoodleUserModel.GetAll().FirstOrDefault(x => x.UserId == userInfo.userId && x.UserName.Equals(userInfo.name)) ?? new MoodleUser()
                                                                                                    {
                                                                                                        Domain = userInfo.domain,
                                                                                                        Password = userInfo.password,
                                                                                                        UserName = userInfo.name,
                                                                                                        UserId = userInfo.userId
                                                                                                    };
            user.Token = token;
            MoodleUserModel.RegisterSave(user);

            string uri = user.Domain + Settings.MoodleUrl;
            var pairs = new NameValueCollection()
            {
                { "wsfunction", "mod_adobeconnect_get_total_quiz_list" },
                { "wstoken", token }
            };
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, pairs);
            }
            string resp = System.Text.Encoding.UTF8.GetString(response);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);

            var quizes = MoodleQuizInfoParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"));

            return new ServiceResponse<MoodleQuizInfoDTO>()
                   {
                       objects = quizes
                   };
        }

        public bool ConvertQuizes(MoodleQuizConvertDTO quiz)
        {
            if (quiz.quizIds == null) return false;

            var user = UserModel.GetOneById(quiz.userId).Value;
            var moodleUser = MoodleUserModel.GetOneByUserId(quiz.userId);

            string uri = moodleUser.Domain + Settings.MoodleUrl;

            foreach (var id in quiz.quizIds)
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
                    response = client.UploadValues(uri, pairs);
                }
                string resp = System.Text.Encoding.UTF8.GetString(response);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resp);

                var q = MoodleQuizParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"));

                this.ConvertAndSave(q, user);
            }
           

            return true;
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

        /// <summary>
        /// The get quizes.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        private MoodleUser ConvertDto(MoodleUserDTO user, MoodleUser instance)
        {
            instance = instance ?? new MoodleUser();
            instance.Password = user.password;
            instance.UserName = user.userName;
            return instance;
        }


        #endregion
    }
}