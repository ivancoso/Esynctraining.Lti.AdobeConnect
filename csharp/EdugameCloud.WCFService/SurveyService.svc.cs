// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;
    using FluentValidation.Results;
    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SurveyService : BaseService, ISurveyService
    {
        #region Properties

        private SurveyModel SurveyModel => IoC.Resolve<SurveyModel>();

        private SurveyGroupingTypeModel SurveyGroupingTypeModel => IoC.Resolve<SurveyGroupingTypeModel>();

        private LmsUserParametersModel LmsUserParametersModel => IoC.Resolve<LmsUserParametersModel>();

        #endregion

        #region Public Methods and Operators
        
        public SurveyDTO Create(SurveySMIWrapperDTO dto)
        {
            if (IsValid(dto, out ValidationResult validationResult))
            {
                var smiResult = Convert(dto.SmiDTO, (SubModuleItem)null, true);
                dto.SurveyDTO.subModuleItemId = smiResult.Id;
                return ConvertQuizAndGetServiceResponse(dto.SurveyDTO, null, SurveyModel);
            }

            var error = GenerateValidationError(validationResult);
            LogError("Survey.Create", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public SurveyDTO Save(SurveyDTO surveyDTO)
        {
            if (IsValid(surveyDTO, out ValidationResult validationResult))
            {
                var model = SurveyModel;
                var isTransient = surveyDTO.surveyId == 0;
                var survey = isTransient ? null : model.GetOneById(surveyDTO.surveyId).Value;
                return ConvertQuizAndGetServiceResponse(surveyDTO, survey, model);
            }

            var error = GenerateValidationError(validationResult);
            LogError("Survey.Create", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public SurveyDTO GetById(int id)
        {
            Survey appletResult;
            if ((appletResult = SurveyModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                LogError("Survey.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SurveyDTO(appletResult);
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        public SurveyDTO GetBySMIId(int id)
        {
            Survey appletResult;
            if ((appletResult = SurveyModel.GetOneBySMIId(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                LogError("Survey.GetBySMIId", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SurveyDTO(appletResult);
        }

        public SurveyFromStoredProcedureDTO[] GetSurveysByUserId(int userId, bool? showLms)
        {
            return SurveyModel.GetSurveysByUserId(userId, showLms ?? false).ToArray();
        }

        public SurveyFromStoredProcedureDTO[] GetLmsSurveys(int userId, int lmsUserParametersId)
        {
            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId).Value;
            return SurveyModel.GetLmsSurveys(userId, lmsUserParameters.Course, lmsUserParameters.CompanyLms.Id).ToArray();
        }

        public SurveyFromStoredProcedureDTO[] GetSharedSurveysByUserId(int userId)
        {
            return SurveyModel.GetSharedForUserSurveysByUserId(userId).ToArray();
        }

        public SMICategoriesFromStoredProcedureDTO[] GetSurveyCategoriesbyUserId(int userId)
        {
            return SurveyModel.GetSurveyCategoriesbyUserId(userId).ToArray();
        }

        public SubModuleItemDTO[] GetSurveySMItemsByUserId(int userId)
        {
            return SubModuleItemModel.GetSurveySubModuleItemsByUserId(userId).ToArray();
        }

        public SurveyDataDTO GetSurveyDataBySurveyId(int surveyId)
        {
            return SurveyModel.GetSurveyDataBySurveyId(surveyId);
        }

        #endregion

        #region Methods

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

        private SurveyDTO ConvertQuizAndGetServiceResponse(
            SurveyDTO surveyDTO,
            Survey survey,
            SurveyModel model)
        {
            survey = ConvertDto(surveyDTO, survey);
            model.RegisterSave(survey, true);
            return new SurveyDTO(survey);
        }

        #endregion

    }

}