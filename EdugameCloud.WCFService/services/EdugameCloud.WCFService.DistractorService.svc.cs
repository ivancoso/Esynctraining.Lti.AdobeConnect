// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
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
    public class DistractorService : BaseService, IDistractorService
    {
        #region Properties

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

        /// <summary>
        /// Gets the sub module item model.
        /// </summary>
        private SubModuleItemModel SubModuleItemModel
        {
            get
            {
                return IoC.Resolve<SubModuleItemModel>();
            }
        }

        /// <summary>
        /// Gets the distractor model.
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
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<DistractorDTO> GetAll()
        {
            return new ServiceResponse<DistractorDTO> { objects = this.DistractorModel.GetAll().Select(x => new DistractorDTO(x)).ToList() };
        }

        /// <summary>
        /// The get by user id and sub module item id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="smiId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<DistractorDTO> GetAllByUserIdAndSubModuleItemId(int userId, int smiId)
        {
            return new ServiceResponse<DistractorDTO> { objects = this.DistractorModel.GetAllByUserIdAndSubModuleItemId(userId, smiId).Select(x => new DistractorDTO(x)).ToList() };
        }

        /// <summary>
        /// The get by user id and question id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="questionId">
        /// The question id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<DistractorDTO> GetAllByUserIdAndQuestionId(int userId, int questionId)
        {
            return new ServiceResponse<DistractorDTO> { objects = this.DistractorModel.GetAllByUserIdAndQuestionId(userId, questionId).Select(x => new DistractorDTO(x)).ToList() };
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
        public ServiceResponse<DistractorDTO> Save(DistractorDTO resultDto)
        {
            var result = new ServiceResponse<DistractorDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var distractorModel = this.DistractorModel;
                var isTransient = resultDto.distractorId == 0;
                var entity = isTransient ? null : distractorModel.GetOneById(resultDto.distractorId).Value;
                entity = ConvertDto(resultDto, entity, true, this.FileModel, this.QuestionModel, this.UserModel, this.SubModuleItemModel);
                distractorModel.RegisterSave(entity);
                result.@object = new DistractorDTO(entity);
                return result;
            }

            result = (ServiceResponse<DistractorDTO>)this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
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
        public ServiceResponse<DistractorDTO> GetById(int id)
        {
            var result = new ServiceResponse<DistractorDTO>();
            Distractor distractor;
            if ((distractor = this.DistractorModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new DistractorDTO(distractor);
            }

            return result;
        }

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
            Distractor entity;
            var model = this.DistractorModel;
            if ((entity = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(entity, true);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        /// The get paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<DistractorDTO> GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<DistractorDTO>
            {
                objects = this.DistractorModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new DistractorDTO(x)).ToList(),
                totalCount = totalCount
            };
        }

        #endregion

        #region Methods

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
        /// The <see cref="Distractor"/>.
        /// </returns>
        internal static Distractor ConvertDto(DistractorDTO q, Distractor instance, bool updateQuestionDate, FileModel fileModel, QuestionModel questionModel, UserModel userModel, SubModuleItemModel subModuleItemModel)
        {
            instance = instance ?? new Distractor();
            instance.IsActive = q.isActive;
            instance.DistractorName = q.distractor;
            instance.DistractorOrder = q.distractorOrder;
            instance.Score = q.score;
            instance.IsCorrect = q.isCorrect;
            instance.DateModified = q.dateModified == DateTime.MinValue ? DateTime.Now : q.dateModified;
            instance.DateCreated = q.dateCreated == DateTime.MinValue ? DateTime.Now : q.dateCreated;
            instance.Image = q.imageId.HasValue ? fileModel.GetOneById(q.imageId.Value).Value : null;
            instance.Question = q.questionId.HasValue ? questionModel.GetOneById(q.questionId.Value).Value : null;
            instance.ModifiedBy = q.modifiedBy.HasValue ? userModel.GetOneById(q.modifiedBy.Value).Value : null;
            instance.CreatedBy = q.createdBy.HasValue ? userModel.GetOneById(q.createdBy.Value).Value : null;
            instance.DistractorType = q.distractorType;
            if (instance.Question != null && updateQuestionDate)
            {
                var subModuleItem = instance.Question.SubModuleItem;
                subModuleItem.DateModified = DateTime.Now;
                subModuleItemModel.RegisterSave(subModuleItem);
            }

            return instance;
        }

        #endregion
    }
}