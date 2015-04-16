// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;

    using FluentValidation.Results;

    using Resources;

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
        ///     The <see cref="SubModuleItemDTO" />.
        /// </returns>
        public SubModuleItemDTO[] GetAll()
        {
            return this.SubModuleItemModel.GetAll().Select(x => new SubModuleItemDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        public SubModuleItemDTO Save(SubModuleItemDTO resultDto)
        {
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var subModuleItemModel = this.SubModuleItemModel;
                var isTransient = resultDto.subModuleItemId == 0;
                var subModuleItem = isTransient ? null : subModuleItemModel.GetOneById(resultDto.subModuleItemId).Value;
                subModuleItem = this.Convert(resultDto, subModuleItem, true);
                return new SubModuleItemDTO(subModuleItem);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("SubModuleItem.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemSaveAllDTO"/>.
        /// </returns>
        public SubModuleItemSaveAllDTO SaveAll(SubModuleItemDTO[] results)
        {
            results = results ?? new SubModuleItemDTO[] { };
            var result = new SubModuleItemSaveAllDTO();
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
                result.saved = created.Select(x => new SubModuleItemDTO(x)).ToArray();
            }

            if (faults.Any())
            {
                result.faults = faults.ToArray();
            }

            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        public SubModuleItemDTO GetById(int id)
        {
            SubModuleItem subModuleItem;
            if ((subModuleItem = this.SubModuleItemModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SubModuleItem.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SubModuleItemDTO(subModuleItem);
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteById(int id)
        {
            SubModuleItem moduleItem;
            var model = this.SubModuleItemModel;
            if ((moduleItem = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("SubModuleItem.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(moduleItem, true);
            return id;
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
        /// The <see cref="PagedSubModuleItemsDTO"/>.
        /// </returns>
        public PagedSubModuleItemsDTO GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedSubModuleItemsDTO
                       {
                           objects =
                               this.SubModuleItemModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new SubModuleItemDTO(x)).ToArray(),
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
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        public SubModuleItemDTO[] GetAppletSubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetAppletSubModuleItemsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get profile sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        public SubModuleItemDTO[] GetSNProfileSubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetSNProfileSubModuleItemsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get quiz sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        public SubModuleItemDTO[] GetQuizSubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetQuizSMItemsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get test sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        public SubModuleItemDTO[] GetTestSubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetTestSubModuleItemsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get survey sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        public SubModuleItemDTO[] GetSurveySubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetSurveySubModuleItemsByUserId(userId).ToArray();
        }

        #endregion
    }
}