namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Domain.Formats;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation.Results;

    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuestionService : ExportService, IQuestionService
    {
        #region Public Methods and Operators

        public QuestionDTO[] GetByUserIdAndSubModuleItemId(int userId, int smiId)
        {
            var questions = this.QuestionModel.GetAllByUserIdAndSubModuleItemId(userId, smiId).ToList();
            var customQuestions = this.QuestionModel.GetCustomQuestionsByQuestionIdsWithTypes(questions.Select(x => new KeyValuePair<int, int>(x.Id, x.QuestionType.Id)));
            return questions.Select(x => new QuestionDTO(x, this.SelectCustomTypeFromList(x, customQuestions))).ToArray();
        }

        public QuestionDTO Save(QuestionDTO dto)
        {
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
                var result = new QuestionDTO(question, customObject);
                if (isTransient && dto.distractors != null && dto.distractors.Any())
                {
                    this.CreateDistractors(dto, question, this.DistractorModel, result);
                }

                return result;
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Question.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// Reorder questions
        /// </summary>
        /// <param name="questions">
        /// Question list with order
        /// </param>
        public void Reorder(QuestionOrderDTO[] questions)
        {
            questions = questions ?? new QuestionOrderDTO[] { };
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
        }

        public QuestionSaveAllDTO SaveAll(QuestionDTO[] results)
        {
            results = results ?? new QuestionDTO[] { };
            var result = new QuestionSaveAllDTO();
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
                        this.CreateDistractors(dto, question, this.DistractorModel, null);
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
                result.saved = created.Select(x => new QuestionDTO(x.Item1, x.Item2)).ToArray();
            }

            if (faults.Any())
            {
                result.faults = faults.ToArray();
            }

            return result;
        }

        public QuestionDTO GetById(int id)
        {
            Question question;
            if ((question = this.QuestionModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("Question.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            var customQuestions = this.QuestionModel.GetCustomQuestionsByQuestionIdsWithTypes(new[] { new KeyValuePair<int, int>(id, question.QuestionType.Id) });
            return new QuestionDTO(question, SelectCustomTypeFromList(question, customQuestions));
        }

        public int DeleteById(int id)
        {
            Question question;
            var model = this.QuestionModel;
            if ((question = model.GetOneById(id).Value) == null)
            {
                var error = 
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound);
                this.LogError("Question.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(question, true);
            return id;
        }

        public string ExportQuestionsBySubModuleItemId(int smiId, int[] questionIds)
        {
            questionIds = questionIds ?? new int[] { };
            return Export(smiId, questionIds.Return(x => x.ToList(), new List<int>()));
        }

        public QuestionDTO[] ImportQuestionsBySubModuleItemId(string id, int smiId, string format)
        {
            return Import(id, smiId, null, (FormatsEnum)Enum.Parse(typeof(FormatsEnum), format));
        }

        public QuestionDTO[] GetParsedQuestionsById(string id, int userId, string format)
        {
            return Import(id, null, userId, (FormatsEnum)Enum.Parse(typeof(FormatsEnum), format));
        }

        public string ExportBySubModuleId(int smiId)
        {
            return Export(smiId);
        }

        #endregion

        #region Methods

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

        private void CreateDistractors(
            QuestionDTO dto,
            Question question,
            DistractorModel distractorModel,
            QuestionDTO result)
        {
            var distractors = new List<DistractorDTO>();
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
                        distractor = isDistractorTransient
                                         ? null
                                         : distractorModel.GetOneById(distractorDTO.distractorId).Value;
                        distractor = DistractorService.ConvertDto(
                            distractorDTO,
                            distractor,
                            false,
                            this.FileModel,
                            this.QuestionModel,
                            this.UserModel,
                            this.SubModuleItemModel);
                        distractorModel.RegisterSave(distractor, true);
                        savedSuccessfully = true;
                    }
                    finally
                    {
                        if (distractor != null && savedSuccessfully && result != null)
                        {
                            distractors.Add(new DistractorDTO(distractor));
                        }
                    }
                }
            }

            if (result != null)
            {
                result.distractors = distractors.ToArray();
            }
        }

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
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            instance.SubModuleItem = q.subModuleItemId.HasValue ? this.SubModuleItemModel.GetOneById(q.subModuleItemId.Value).Value : null;
            instance.Image = q.imageId.HasValue ? this.FileModel.GetOneById(q.imageId.Value).Value : null;
            instance.QuestionType = this.QuestionTypeModel.GetOneById(q.questionTypeId).Value;
            instance.ModifiedBy = q.modifiedBy.HasValue ? this.UserModel.GetOneById(q.modifiedBy.Value).Value : null;
            instance.CreatedBy = q.createdBy.HasValue ? this.UserModel.GetOneById(q.createdBy.Value).Value : null;
            instance.RandomizeAnswers = q.randomizeAnswers ?? (q.questionTypeId == (int)QuestionTypeEnum.Sequence
                                            ? (bool?)true
                                            : null);
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