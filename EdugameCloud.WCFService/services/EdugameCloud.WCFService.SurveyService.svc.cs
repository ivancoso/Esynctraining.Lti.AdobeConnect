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
    using EdugameCloud.Core.Domain.Formats.Edugame;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SurveyService : BaseService, ISurveyService
    {
        #region Properties

        /// <summary>
        /// Gets the sub module category model.
        /// </summary>
        private SubModuleCategoryModel SubModuleCategoryModel
        {
            get
            {
                return IoC.Resolve<SubModuleCategoryModel>();
            }
        }

        /// <summary>
        /// Gets the Survey model.
        /// </summary>
        private SurveyModel SurveyModel
        {
            get
            {
                return IoC.Resolve<SurveyModel>();
            }
        }

        /// <summary>
        /// Gets the SurveyGroupingType model.
        /// </summary>
        private SurveyGroupingTypeModel SurveyGroupingTypeModel
        {
            get
            {
                return IoC.Resolve<SurveyGroupingTypeModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All items test.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<SurveyDTO> GetAll()
        {
            return new ServiceResponse<SurveyDTO> { objects = this.SurveyModel.GetAll().Select(x => new SurveyDTO(x)).ToList() };
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
        public ServiceResponse<SurveyDTO> GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<SurveyDTO>
            {
                objects = this.SurveyModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new SurveyDTO(x)).ToList(),
                totalCount = totalCount
            };
        }

        /// <summary>
        /// The creation of quiz.
        /// </summary>
        /// <param name="dto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyDTO> Create(SurveySMIWrapperDTO dto)
        {
            var result = new ServiceResponse<SurveyDTO>();
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var quizModel = this.SurveyModel;
                var smiResult = this.ConvertDto(dto.SmiDTO, null);
                this.SubModuleItemModel.RegisterSave(smiResult, true);
                dto.SurveyDTO.subModuleItemId = smiResult.Id;
                return this.ConvertQuizAndGetServiceResponse(dto.SurveyDTO, null, quizModel, result);
            }

            result = (ServiceResponse<SurveyDTO>)this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="surveyDTO">
        /// The survey.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyDTO> Save(SurveyDTO surveyDTO)
        {
            var result = new ServiceResponse<SurveyDTO>();
            ValidationResult validationResult;
            if (this.IsValid(surveyDTO, out validationResult))
            {
                var quizModel = this.SurveyModel;
                var isTransient = surveyDTO.surveyId == 0;
                var survey = isTransient ? null : quizModel.GetOneById(surveyDTO.surveyId).Value;
                return this.ConvertQuizAndGetServiceResponse(surveyDTO, survey, quizModel, result);
            }

            result = (ServiceResponse<SurveyDTO>)this.UpdateResult(result, validationResult);
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
        public ServiceResponse<SurveyDTO> GetById(int id)
        {
            var result = new ServiceResponse<SurveyDTO>();
            Survey appletResult;
            if ((appletResult = this.SurveyModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SurveyDTO(appletResult);
            }

            return result;
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyDTO> GetBySMIId(int id)
        {
            var result = new ServiceResponse<SurveyDTO>();
            Survey appletResult;
            if ((appletResult = this.SurveyModel.GetOneBySMIId(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SurveyDTO(appletResult);
            }

            return result;
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyFromStoredProcedureDTO> GetSurveysByUserId(int userId)
        {
            return new ServiceResponse<SurveyFromStoredProcedureDTO> { objects = this.SurveyModel.GetSurveysByUserId(userId).ToList() };
        }

        /// <summary>
        /// The get by user id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyFromStoredProcedureDTO> GetSharedSurveysByUserId(int userId)
        {
            return new ServiceResponse<SurveyFromStoredProcedureDTO> { objects = this.SurveyModel.GetSharedForUserSurveysByUserId(userId).ToList() };
        }

        /// <summary>
        /// The get quiz categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SMICategoriesFromStoredProcedureDTO> GetSurveyCategoriesbyUserId(int userId)
        {
            return new ServiceResponse<SMICategoriesFromStoredProcedureDTO> { objects = this.SurveyModel.GetSurveyCategoriesbyUserId(userId).ToList() };
        }

        /// <summary>
        /// The get survey sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleItemDTOFromStoredProcedureDTO> GetSurveySMItemsByUserId(int userId)
        {
            return new ServiceResponse<SubModuleItemDTOFromStoredProcedureDTO> { objects = this.SurveyModel.GetSurveySMItemsByUserId(userId).ToList() };
        }

        /// <summary>
        /// The get quiz data by quiz id.
        /// </summary>
        /// <param name="surveyId">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyDataDTO> GetSurveyDataBySurveyId(int surveyId)
        {
            return new ServiceResponse<SurveyDataDTO> { @object = this.SurveyModel.GetSurveyDataBySurveyId(surveyId) };
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="itemDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="Survey"/>.
        /// </returns>
        private Survey ConvertDto(SurveyDTO itemDTO, Survey instance)
        {
            instance = instance ?? new Survey();
            instance.SurveyName = itemDTO.surveyName;
            instance.Description = itemDTO.description;
            instance.SubModuleItem = itemDTO.subModuleItemId.HasValue ? this.SubModuleItemModel.GetOneById(itemDTO.subModuleItemId.Value).Value : null;
            instance.SurveyGroupingType = this.SurveyGroupingTypeModel.GetOneById(itemDTO.surveyGroupingTypeId).Value;
            if (instance.SubModuleItem != null)
            {
                instance.SubModuleItem.DateModified = DateTime.Now;
                this.SubModuleItemModel.RegisterSave(instance.SubModuleItem);
            }

            return instance;
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="smi">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItem"/>.
        /// </returns>
        private SubModuleItem ConvertDto(SubModuleItemDTO smi, SubModuleItem instance)
        {
            instance = instance ?? new SubModuleItem();
            instance.IsActive = smi.isActive;
            instance.IsShared = smi.isShared;
            instance.DateCreated = smi.dateCreated == DateTime.MinValue ? DateTime.Now : smi.dateCreated;
            instance.DateModified = smi.dateModified == DateTime.MinValue ? DateTime.Now : smi.dateModified;
            instance.SubModuleCategory = this.SubModuleCategoryModel.GetOneById(smi.subModuleCategoryId).Value;
            instance.CreatedBy = smi.createdBy.HasValue ? this.UserModel.GetOneById(smi.createdBy.Value).Value : null;
            instance.ModifiedBy = smi.modifiedBy.HasValue ? this.UserModel.GetOneById(smi.modifiedBy.Value).Value : null;
            return instance;
        }

        /// <summary>
        /// The convert quiz and get service response.
        /// </summary>
        /// <param name="surveyDTO">
        /// The survey DTO.
        /// </param>
        /// <param name="survey">
        /// The survey.
        /// </param>
        /// <param name="quizModel">
        /// The quiz model.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private ServiceResponse<SurveyDTO> ConvertQuizAndGetServiceResponse(
            SurveyDTO surveyDTO,
            Survey survey,
            SurveyModel quizModel,
            ServiceResponse<SurveyDTO> result)
        {
            survey = this.ConvertDto(surveyDTO, survey);
            quizModel.RegisterSave(survey, true);
            int companyId =
                survey.With(x => x.SubModuleItem).With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
            IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<Survey>(NotificationType.Update, companyId, survey.Id);
            result.@object = new SurveyDTO(survey);
            return result;
        }

        #endregion
    }
}