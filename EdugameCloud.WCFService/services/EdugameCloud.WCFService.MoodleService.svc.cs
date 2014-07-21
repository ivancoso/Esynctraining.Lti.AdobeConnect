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

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private ServiceResponse<MoodleUserDTO> Save(MoodleUserDTO user)
        {
            var result = new ServiceResponse<MoodleUserDTO>();
            ValidationResult validationResult;
            if (this.IsValid(user, out validationResult))
            {
                MoodleUserModel userModel = this.MoodleUserModel;
                bool isTransient = user.moodleUserId == 0;
                MoodleUser userInstance = isTransient ? null : userModel.GetOneById(user.moodleUserId).Value;
                userInstance = this.ConvertDto(user, userInstance);
                userModel.RegisterSave(userInstance, true);
                result.@object = new MoodleUserDTO(userInstance);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
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
        public ServiceResponse<MoodleQuizInfoDTO> GetQuizesForUser(string token)
        {
            string uri = Settings.MoodleUrl;
            var pairs = new NameValueCollection()
            {
                { "wsfunction", "mod_adobeconnect_get_total_quiz_list" },
                { "wstoken", string.IsNullOrWhiteSpace(token) ? Settings.MoodleDefaultToken : token }
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

        public bool ConvertQuizes(List<int> ids)
        {
            string uri = Settings.MoodleUrl;

            foreach (var id in ids)
            {
                var pairs = new NameValueCollection()
                {
                    { "wsfunction", "mod_adobeconnect_get_quiz_by_id" },
                    { "wstoken", Settings.MoodleDefaultToken },
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

                var quiz = MoodleQuizParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"));

                this.ConvertAndSave(quiz);
            }
           

            return true;
        }

        private void ConvertAndSave(MoodleQuiz quiz)
        {
            var findQuiz =
                QuizModel.GetAll().FirstOrDefault(q => q.MoodleId.ToString().Equals(quiz.Id ?? "-1"));

            var egcQuiz = findQuiz ?? new Quiz();
            
            var submodule = findQuiz != null ? findQuiz.SubModuleItem : new SubModuleItem();
            submodule.IsActive = true;
            submodule.IsShared = true;
            submodule.DateModified = DateTime.Now;
            submodule.DateCreated = DateTime.Now;
            submodule.CreatedBy = UserModel.GetOneByEmail("demo@esynctraining.com").Value;
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

            foreach (var q in quiz.Questions.Where(qs => qs.QuestionType != null && qs.QuestionType.Equals("truefalse")))
            {
                var question = new Question();

                question.SubModuleItem = submodule;
                question.QuestionName = Regex.Replace(q.QuestionText, "<[^>]*(>|$)", "");
                question.QuestionType = QuestionTypeModel.GetOneById(2).Value;
                question.DateModified = DateTime.Now;
                question.DateCreated = DateTime.Now;
                question.CreatedBy = UserModel.GetOneByEmail("demo@esynctraining.com").Value;
                question.ModifiedBy = UserModel.GetOneByEmail("demo@esynctraining.com").Value;
                question.IsActive = true;
                
                QuestionModel.RegisterSave(question);

                var qtf = new QuestionForTrueFalse();
                qtf.Question = question;

                QuestionForTrueFalseModel.RegisterSave(qtf);

                var distractor = new Distractor();
                distractor.DateModified = DateTime.Now;
                distractor.DateCreated = DateTime.Now;
                distractor.CreatedBy = UserModel.GetOneByEmail("demo@esynctraining.com").Value;
                distractor.ModifiedBy = UserModel.GetOneByEmail("demo@esynctraining.com").Value;
                distractor.Question = question;
                distractor.DistractorName = "truefalse";
                distractor.IsActive = true;
                distractor.DistractorType = 1;

                bool isTrue = false;
                foreach (var a in q.Answers)
                {
                    if (a.Answer != null && a.Answer.ToLower().Equals("true") && a.Fraction != null
                        && a.Fraction.StartsWith("1")) isTrue = true;
                }
                distractor.IsCorrect = isTrue;

                DistractorModel.RegisterSave(distractor);
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
            instance.CompanyId = user.companyId;
            instance.FirstName = user.firstName;
            instance.MoodleUserId = user.moodleUserId;
            instance.LastName = user.lastName;
            instance.Password = user.password;
            instance.UserName = user.userName;
            return instance;
        }


        #endregion
    }
}