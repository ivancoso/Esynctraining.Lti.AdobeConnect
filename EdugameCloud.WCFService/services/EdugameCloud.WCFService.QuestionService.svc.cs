// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Domain.Formats;
    using EdugameCloud.WCFService.Base;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuestionService : ExportService, IQuestionService
    {
        #region Public Methods and Operators

        /// <summary>
        /// Get by user subModuleId and sub module item subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="smiId">
        /// The sub module item Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuestionDTO> GetByUserIdAndSubModuleItemId(int userId, int smiId)
        {
            var questions = this.QuestionModel.GetAllByUserIdAndSubModuleItemId(userId, smiId).ToList();
            var customQuestions = this.QuestionModel.GetCustomQuestionsByQuestionIdsWithTypes(questions.Select(x => new KeyValuePair<int, int>(x.Id, x.QuestionType.Id)));
            return new ServiceResponse<QuestionDTO> { objects = questions.Select(x => new QuestionDTO(x, this.SelectCustomTypeFromList(x, customQuestions))).ToList() };
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuestionDTO> Save(QuestionDTO dto)
        {
            var result = new ServiceResponse<QuestionDTO>();
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var questionModel = this.QuestionModel;
                var isTransient = dto.questionId == 0;
                var question = isTransient ? null : questionModel.GetOneById(dto.questionId).Value;
                question = this.ConvertDto(dto, question);
                bool wasTransient = question.IsTransient();
                questionModel.RegisterSave(question, true);
                var customObject = this.ProcessCustomQuestions(question, dto, wasTransient);
                result.@object = new QuestionDTO(question, customObject);
                if (isTransient && dto.distractors != null && dto.distractors.Any())
                {
                    this.CreateDistractors(dto, question, this.DistractorModel, result);
                }

                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// Reorder questions
        /// </summary>
        /// <param name="questions">Question list with order</param>
        /// <returns>
        /// Service response
        /// </returns>
        public ServiceResponse Reorder(List<QuestionOrderDTO> questions)
        {
            var result = new ServiceResponse();
            var questionModel = QuestionModel;
            foreach (var dto in questions)
            {
                var isTransient = dto.questionId == 0;
                var question = isTransient ? null : questionModel.GetOneById(dto.questionId).Value;
                if (question != null)
                {
                    question.QuestionOrder = dto.questionOrder;
                    questionModel.RegisterSave(question);
                }
            }

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
        public ServiceResponse<QuestionDTO> SaveAll(List<QuestionDTO> results)
        {
            var result = new ServiceResponse<QuestionDTO>();
            var faults = new List<string>();
            var created = new List<Tuple<Question, QuestionFor>>();
            foreach (var dto in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(dto, out validationResult))
                {
                    var questionModel = this.QuestionModel;
                    var isTransient = dto.questionId == 0;
                    var question = isTransient ? null : questionModel.GetOneById(dto.questionId).Value;
                    question = this.ConvertDto(dto, question);
                    questionModel.RegisterSave(question, true);
                    if (isTransient && dto.distractors != null && dto.distractors.Any())
                    {
                        this.CreateDistractors(dto, question, this.DistractorModel, result);
                    }

                    var customObject = this.ProcessCustomQuestions(question, dto, isTransient);

                    created.Add(new Tuple<Question, QuestionFor>(question, customObject));
                }
                else
                {
                    faults.AddRange(this.UpdateResultToString(validationResult));
                }
            }

            if (created.Any())
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new QuestionDTO(x.Item1, x.Item2)).ToList();
            }

            if (faults.Any())
            {
                result.status = created.Any() ? Errors.CODE_RESULTTYPE_PARTIALSUCCESS : Errors.CODE_RESULTTYPE_ERROR;
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.EntityCreationError_Subject, ErrorsTexts.EntityCreation_PartialSuccessMessage, faults));
            }

            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The get by subModuleId.
        /// </summary>
        /// <param name="id">
        /// The subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuestionDTO> GetById(int id)
        {
            var result = new ServiceResponse<QuestionDTO>();
            Question question;
            if ((question = this.QuestionModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                var customQuestions = this.QuestionModel.GetCustomQuestionsByQuestionIdsWithTypes(new[] { new KeyValuePair<int, int>(id, question.QuestionType.Id) });
                result.@object = new QuestionDTO(question, this.SelectCustomTypeFromList(question, customQuestions));
            }

            return result;
        }

        /// <summary>
        /// The delete by subModuleId.
        /// </summary>
        /// <param name="id">
        /// The subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> DeleteById(int id)
        {
            var result = new ServiceResponse<int>();
            Question question;
            var model = this.QuestionModel;
            if ((question = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(question, true);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        /// Export questions by SubModule item id.
        /// </summary>
        /// <param name="smiId">SubModule item id.</param>
        /// <param name="questionIds">Question ids.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        public ServiceResponse<string> ExportQuestionsBySubModuleItemId(int smiId, List<int> questionIds)
        {
            return this.Export(smiId, questionIds);
        }

        /// <summary>
        /// Import questions by SubModule item id.
        /// </summary>
        /// <param name="id">Import id.</param>
        /// <param name="smiId">SubModule item id.</param>
        /// <param name="format">Import format.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        public ServiceResponse<QuestionDTO> ImportQuestionsBySubModuleItemId(string id, int smiId, string format)
        {
            return this.Import(id, smiId, null, (FormatsEnum)Enum.Parse(typeof(FormatsEnum), format));
        }

        /// <summary>
        /// Import questions by SubModule item id.
        /// </summary>
        /// <param name="id">
        /// Import id.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="format">
        /// Import format.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuestionDTO> GetParsedQuestionsById(string id, int userId, string format)
        {
            return this.Import(id, null, userId, (FormatsEnum)Enum.Parse(typeof(FormatsEnum), format));
        }

        /// <summary>
        /// Export by SubModule id.
        /// </summary>
        /// <param name="smiId">SubModule id.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        public ServiceResponse<string> ExportBySubModuleId(int smiId)
        {
            return this.Export(smiId);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The process custom questions.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <param name="wasTransient">
        /// The was transient.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionFor"/>.
        /// </returns>
        private QuestionFor ProcessCustomQuestions(Question question, QuestionDTO dto, bool wasTransient)
        {
            switch (question.QuestionType.Id)
            {
                case (int)QuestionTypeEnum.TrueFalse:
                    var qtf = wasTransient ? new QuestionForTrueFalse() : question.TrueFalseQuestions.FirstOrDefault() ?? new QuestionForTrueFalse();
                    qtf.PageNumber = dto.pageNumber;
                    qtf.IsMandatory = dto.isMandatory;
                    qtf.Question = question;
                    this.QuestionForTrueFalseModel.RegisterSave(qtf);
                    return qtf;
                case (int)QuestionTypeEnum.Rate:
                    var qr = wasTransient ? new QuestionForRate() : question.RateQuestions.FirstOrDefault() ?? new QuestionForRate();
                    qr.AllowOther = dto.allowOther;
                    qr.Restrictions = dto.restrictions;
                    qr.PageNumber = dto.pageNumber;
                    qr.IsMandatory = dto.isMandatory;
                    qr.Question = question;
                    this.QuestionForRateModel.RegisterSave(qr);
                    return qr;

                case (int)QuestionTypeEnum.OpenAnswerMultiLine:
                case (int)QuestionTypeEnum.OpenAnswerSingleLine:
                    var qoa = wasTransient ? new QuestionForOpenAnswer() : question.OpenAnswerQuestions.FirstOrDefault() ?? new QuestionForOpenAnswer();
                    qoa.Restrictions = dto.restrictions;
                    qoa.PageNumber = dto.pageNumber;
                    qoa.IsMandatory = dto.isMandatory;
                    qoa.Question = question;
                    this.QuestionForOpenAnswerModel.RegisterSave(qoa);
                    return qoa;

                case (int)QuestionTypeEnum.RateScaleLikert:
                    var ql = wasTransient ? new QuestionForLikert() : question.LikertQuestions.FirstOrDefault() ?? new QuestionForLikert();
                    ql.AllowOther = dto.allowOther;
                    ql.PageNumber = dto.pageNumber;
                    ql.IsMandatory = dto.isMandatory;
                    ql.Question = question;
                    this.QuestionForLikertModel.RegisterSave(ql);
                    return ql;

                case (int)QuestionTypeEnum.SingleMultipleChoiceImage:
                case (int)QuestionTypeEnum.SingleMultipleChoiceText:
                    var qc = wasTransient ? new QuestionForSingleMultipleChoice() : question.SingleMultipleChoiceQuestions.FirstOrDefault() ?? new QuestionForSingleMultipleChoice();
                    qc.AllowOther = dto.allowOther;
                    qc.PageNumber = dto.pageNumber;
                    qc.IsMandatory = dto.isMandatory;
                    qc.Restrictions = dto.restrictions;
                    qc.Question = question;
                    this.QuestionForSingleMultipleChoiceModel.RegisterSave(qc);
                    return qc;

                case (int)QuestionTypeEnum.WeightedBucketRatio:
                    var qw = wasTransient ? new QuestionForWeightBucket() : question.WeightBucketQuestions.FirstOrDefault() ?? new QuestionForWeightBucket();
                    qw.AllowOther = dto.allowOther;
                    qw.PageNumber = dto.pageNumber;
                    qw.TotalWeightBucket = dto.totalWeightBucket;
                    qw.WeightBucketType = dto.weightBucketType;
                    qw.IsMandatory = dto.isMandatory;
                    qw.Question = question;
                    this.QuestionForWeightBucketModel.RegisterSave(qw);
                    return qw;
            }

            return null;
        }

/// <summary>
        /// The create distractors.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractorModel">
        /// The distractor model.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        private void CreateDistractors(QuestionDTO dto, Question question, DistractorModel distractorModel, ServiceResponse<QuestionDTO> result)
        {
            foreach (var distractorDTO in dto.distractors)
            {
                distractorDTO.questionId = question.Id;
                ValidationResult distractorValidationResult;
                if (this.IsValid(distractorDTO, out distractorValidationResult))
                {
                    Distractor distractor = null;
                    bool savedSuccessfully = false;
                    try
                    {
                        var isDistractorTransient = distractorDTO.distractorId == 0;
                        distractor = isDistractorTransient ? null : distractorModel.GetOneById(distractorDTO.distractorId).Value;
                        distractor = DistractorService.ConvertDto(distractorDTO, distractor, false, this.FileModel, this.QuestionModel, this.UserModel, this.SubModuleItemModel);
                        distractorModel.RegisterSave(distractor, true);
                        savedSuccessfully = true;
                    }
                    finally
                    {
                        if (distractor != null && savedSuccessfully && result.@object != null)
                        {
                            result.@object.distractors = result.@object.distractors ?? new List<DistractorDTO>();
                            result.@object.distractors.Add(new DistractorDTO(distractor));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="q">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategory"/>.
        /// </returns>
        private Question ConvertDto(QuestionDTO q, Question instance)
        {
            instance = instance ?? new Question();
            instance.IsActive = true;
            instance.QuestionOrder = q.questionOrder;
            instance.QuestionName = q.question;
            instance.Hint = q.hint;
            instance.IncorrectMessage = q.incorrectMessage;
            instance.Instruction = q.instruction;
            instance.CorrectMessage = q.correctMessage;
            instance.CorrectReference = q.correctReference;
            instance.ScoreValue = q.scoreValue;
            instance.DateModified = q.dateModified == DateTime.MinValue ? DateTime.Now : q.dateModified;
            instance.DateCreated = q.dateCreated == DateTime.MinValue ? DateTime.Now : q.dateCreated;
            instance.SubModuleItem = q.subModuleItemId.HasValue ? this.SubModuleItemModel.GetOneById(q.subModuleItemId.Value).Value : null;
            instance.Image = q.imageId.HasValue ? this.FileModel.GetOneById(q.imageId.Value).Value : null;
            instance.QuestionType = this.QuestionTypeModel.GetOneById(q.questionTypeId).Value;
            instance.ModifiedBy = q.modifiedBy.HasValue ? this.UserModel.GetOneById(q.modifiedBy.Value).Value : null;
            instance.CreatedBy = q.createdBy.HasValue ? this.UserModel.GetOneById(q.createdBy.Value).Value : null;
            instance.RandomizeAnswers = q.randomizeAnswers;
            if (instance.SubModuleItem != null)
            {
                instance.SubModuleItem.DateModified = DateTime.Now;
                this.SubModuleItemModel.RegisterSave(instance.SubModuleItem);
            }

            return instance;
        }

        #endregion
    }
}