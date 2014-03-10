// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
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
    public class SubModuleItemService : BaseService, ISubModuleItemService
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Get all.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<SubModuleItemDTO> GetAll()
        {
            return new ServiceResponse<SubModuleItemDTO> { objects = this.SubModuleItemModel.GetAll().Select(x => new SubModuleItemDTO(x)).ToList() };
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
        public ServiceResponse<SubModuleItemDTO> Save(SubModuleItemDTO resultDto)
        {
            var result = new ServiceResponse<SubModuleItemDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var subModuleItemModel = this.SubModuleItemModel;
                var isTransient = resultDto.subModuleItemId == 0;
                var subModuleItem = isTransient ? null : subModuleItemModel.GetOneById(resultDto.subModuleItemId).Value;
                subModuleItem = this.Convert(resultDto, subModuleItem, true);
                result.@object = new SubModuleItemDTO(subModuleItem);
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
        public ServiceResponse<SubModuleItemDTO> SaveAll(List<SubModuleItemDTO> results)
        {
            var result = new ServiceResponse<SubModuleItemDTO>();
            var faults = new List<string>();
            var created = new List<SubModuleItem>();
            foreach (var appletResultDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(appletResultDTO, out validationResult))
                {
                    var sessionModel = this.SubModuleItemModel;
                    var isTransient = appletResultDTO.subModuleItemId == 0;
                    var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.subModuleItemId).Value;
                    appletResult = this.Convert(appletResultDTO, appletResult);
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
                result.objects = created.Select(x => new SubModuleItemDTO(x)).ToList();
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
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleItemDTO> GetById(int id)
        {
            var result = new ServiceResponse<SubModuleItemDTO>();
            SubModuleItem subModuleItem;
            if ((subModuleItem = this.SubModuleItemModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SubModuleItemDTO(subModuleItem);
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
            SubModuleItem moduleItem;
            var model = this.SubModuleItemModel;
            if ((moduleItem = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(moduleItem, true);
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
        public ServiceResponse<SubModuleItemDTO> GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<SubModuleItemDTO>
                       {
                           objects =
                               this.SubModuleItemModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new SubModuleItemDTO(x)).ToList(),
                            totalCount = totalCount
                       };
        }

        /// <summary>
        /// The get applet sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleItemDTO> GetAppletSubModuleItemsByUserId(int userId)
        {
            return new ServiceResponse<SubModuleItemDTO>
                       {
                           objects = this.SubModuleItemModel.GetAppletSubModuleItemsByUserId(userId)
                       };
        }

        /// <summary>
        /// The get profile sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleItemDTO> GetSNProfileSubModuleItemsByUserId(int userId)
        {
            return new ServiceResponse<SubModuleItemDTO>
            {
                objects = this.SubModuleItemModel.GetSNProfileSubModuleItemsByUserId(userId)
            };
        }

        /// <summary>
        /// The get quiz sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleItemDTO> GetQuizSubModuleItemsByUserId(int userId)
        {
            return new ServiceResponse<SubModuleItemDTO>
            {
                objects = this.SubModuleItemModel.GetQuizSMItemsByUserId(userId).ToList()
            };
        }

        /// <summary>
        /// The get test sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleItemDTO> GetTestSubModuleItemsByUserId(int userId)
        {
            return new ServiceResponse<SubModuleItemDTO>
            {
                objects = this.SubModuleItemModel.GetTestSubModuleItemsByUserId(userId)
            };
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
        public ServiceResponse<SubModuleItemDTO> GetSurveySubModuleItemsByUserId(int userId)
        {
            return new ServiceResponse<SubModuleItemDTO>
            {
                objects = this.SubModuleItemModel.GetSurveySubModuleItemsByUserId(userId)
            };
        }

        #endregion
    }
}