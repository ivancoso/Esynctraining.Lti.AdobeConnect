// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web.Script.Serialization;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuizQuestionResultService : BaseService, IQuizQuestionResultService
    {
        #region Properties

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        private MoodleUserModel MoodleUserModel
        {
            get
            {
                return IoC.Resolve<MoodleUserModel>();
            }
        }

        /// <summary>
        ///     Gets the question model.
        /// </summary>
        private QuestionModel QuestionModel
        {
            get
            {
                return IoC.Resolve<QuestionModel>();
            }
        }

        /// <summary>
        ///     Gets the question type model.
        /// </summary>
        private QuestionTypeModel QuestionTypeModel
        {
            get
            {
                return IoC.Resolve<QuestionTypeModel>();
            }
        }

        /// <summary>
        ///     Gets the quiz question result model.
        /// </summary>
        private QuizQuestionResultModel QuizQuestionResultModel
        {
            get
            {
                return IoC.Resolve<QuizQuestionResultModel>();
            }
        }

        /// <summary>
        ///     Gets the quiz result model.
        /// </summary>
        private QuizResultModel QuizResultModel
        {
            get
            {
                return IoC.Resolve<QuizResultModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> DeleteById(int id)
        {
            var result = new ServiceResponse<int>();
            QuizQuestionResult quizResult;
            QuizQuestionResultModel model = this.QuizQuestionResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(quizResult, true);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        ///     Gets all quiz questions results.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<QuizQuestionResultDTO> GetAll()
        {
            return new ServiceResponse<QuizQuestionResultDTO>
                       {
                           objects =
                               this.QuizQuestionResultModel.GetAll()
                               .Select(x => new QuizQuestionResultDTO(x))
                               .ToList()
                       };
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizQuestionResultDTO> GetById(int id)
        {
            var result = new ServiceResponse<QuizQuestionResultDTO>();
            QuizQuestionResult quizResult;
            if ((quizResult = this.QuizQuestionResultModel.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new QuizQuestionResultDTO(quizResult);
            }

            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizQuestionResultDTO> Save(QuizQuestionResultDTO resultDto)
        {
            var result = new ServiceResponse<QuizQuestionResultDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                QuizQuestionResultModel quizQuestionResultModel = this.QuizQuestionResultModel;
                bool isTransient = resultDto.quizQuestionResultId == 0;
                QuizQuestionResult quizQuestionResult = isTransient
                                                            ? null
                                                            : quizQuestionResultModel.GetOneById(
                                                                resultDto.quizQuestionResultId).Value;
                quizQuestionResult = this.ConvertDto(resultDto, quizQuestionResult);
                quizQuestionResultModel.RegisterSave(quizQuestionResult);
                result.@object = new QuizQuestionResultDTO(quizQuestionResult);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizQuestionResultDTO> SaveAll(List<QuizQuestionResultDTO> results)
        {
            var result = new ServiceResponse<QuizQuestionResultDTO>();
            var faults = new List<string>();
            var created = new List<QuizQuestionResult>();
            foreach (QuizQuestionResultDTO appletResultDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(appletResultDTO, out validationResult))
                {
                    QuizQuestionResultModel sessionModel = this.QuizQuestionResultModel;
                    bool isTransient = appletResultDTO.quizQuestionResultId == 0;
                    QuizQuestionResult appletResult = isTransient
                                                          ? null
                                                          : sessionModel.GetOneById(
                                                              appletResultDTO.quizQuestionResultId).Value;
                    appletResult = this.ConvertDto(appletResultDTO, appletResult);
                    sessionModel.RegisterSave(appletResult);
                    created.Add(appletResult);
                }
                else
                {
                    faults.AddRange(this.UpdateResultToString(validationResult));
                }
            }

            if (created.Any())
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new QuizQuestionResultDTO(x)).ToList();
            }

            if (faults.Any())
            {
                result.status = created.Any() ? Errors.CODE_RESULTTYPE_PARTIALSUCCESS : Errors.CODE_RESULTTYPE_ERROR;
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.EntityCreationError_Subject, 
                        ErrorsTexts.EntityCreation_PartialSuccessMessage, 
                        faults));
            }

            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);

            this.SendResultsToMoodle(results);

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="resultDTO">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="QuizQuestionResult"/>.
        /// </returns>
        private QuizQuestionResult ConvertDto(QuizQuestionResultDTO resultDTO, QuizQuestionResult instance)
        {
            instance = instance ?? new QuizQuestionResult();
            instance.Question = resultDTO.question;
            instance.IsCorrect = resultDTO.isCorrect;
            instance.QuestionType = this.QuestionTypeModel.GetOneById(resultDTO.questionTypeId).Value;
            instance.QuizResult = this.QuizResultModel.GetOneById(resultDTO.quizResultId).Value;
            instance.QuestionRef = this.QuestionModel.GetOneById(resultDTO.questionId).Value;
            return instance;
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
        /// The send results to moodle.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        private void SendResultsToMoodle(IEnumerable<QuizQuestionResultDTO> results)
        {
            var toSend = new List<MoodleQuizResultDTO>();

            foreach (QuizQuestionResultDTO r in results)
            {
                var m = new MoodleQuizResultDTO();
                QuizResult quizResult = this.QuizResultModel.GetOneById(r.quizResultId).Value;
                m.quizId = quizResult.Quiz.LmsQuizId ?? 0;
                Question question = this.QuestionModel.GetOneById(r.questionId).Value;
                m.questionId = question.LmsQuestionId ?? 0;
                m.questionType = question.QuestionType.MoodleQuestionType;
                m.isSingle = question.IsMoodleSingle.GetValueOrDefault();
                m.userId = quizResult.LmsId;
                m.startTime = quizResult.StartTime.ConvertToUTCTimestamp();

                switch (question.QuestionType.Id)
                {
                    case (int)QuestionTypeEnum.TrueFalse:
                        Distractor distractor = question.Distractors != null
                                                    ? question.Distractors.FirstOrDefault()
                                                    : null;
                        if (distractor != null)
                        {
                            bool answer = (r.isCorrect && distractor.IsCorrect.GetValueOrDefault())
                                           || (!r.isCorrect && !distractor.IsCorrect.GetValueOrDefault());
                            m.answers = new List<string> { answer ? "true" : "false" };
                        }

                        break;
                    case (int)QuestionTypeEnum.CalculatedMultichoice:
                    case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                        m.answers =
                            question.Distractors.Where(q => r.answers != null && r.answers.Contains(q.Id.ToString(CultureInfo.InvariantCulture)))
                                .Select(q => q.LmsAnswer)
                                .ToList();
                        break;
                    case (int)QuestionTypeEnum.Matching:
                        var userAnswers = new Dictionary<string, string>();
                        if (r.answers != null)
                        {
                            r.answers.ForEach(
                                answer =>
                                    {
                                        int splitInd = answer.IndexOf("$$", System.StringComparison.Ordinal);
                                        if (splitInd > -1)
                                        {
                                            string left = answer.Substring(0, splitInd), 
                                                   right = answer.Substring(splitInd + 2, answer.Length - splitInd - 2);
                                            if (!userAnswers.ContainsKey(left))
                                            {
                                                userAnswers.Add(left, right);
                                            }
                                        }
                                    });
                        }

                        m.answers = new List<string>();
                        foreach (Distractor d in question.Distractors.OrderBy(ds => ds.LmsAnswerId))
                        {
                            string key = d.DistractorName.Substring(0, d.DistractorName.IndexOf("$$", System.StringComparison.Ordinal));
                            m.answers.Add(userAnswers.ContainsKey(key) ? userAnswers[key] : string.Empty);
                        }

                        break;
                    default:
                        m.answers = r.answers;
                        break;
                }

                if (m.userId > 0 & m.quizId > 0)
                {
                    toSend.Add(m);
                }
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

            string json = new JavaScriptSerializer().Serialize(ret);

            MoodleUser moodleUser =
                this.MoodleUserModel.GetAll().OrderByDescending(u => u.DateModified).FirstOrDefault();
            if (moodleUser == null)
            {
                return;
            }

            var pairs = new NameValueCollection
                            {
                                { "wsfunction", "mod_adobeconnect_save_external_quiz_report" }, 
                                { "wstoken", moodleUser.Token }, 
                                { "reportObject", json }
                            };

            using (var client = new WebClient())
            {
                client.UploadValues(this.GetServicesUrl(moodleUser.Domain), pairs);
            }
        }

        #endregion
    }
}