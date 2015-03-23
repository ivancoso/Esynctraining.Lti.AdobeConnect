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
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

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
    public class SubModuleCategoryService : BaseService, ISubModuleCategoryService
    {
        #region Properties

        /// <summary>
        /// Gets the sub module model.
        /// </summary>
        private SubModuleModel SubModuleModel
        {
            get
            {
                return IoC.Resolve<SubModuleModel>();
            }
        }

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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All test.
        /// </summary>
        /// <returns>
        ///     The <see cref="SubModuleCategoryDTO" />.
        /// </returns>
        public SubModuleCategoryDTO[] GetAll()
        {
            return this.SubModuleCategoryModel.GetAll().Select(x => new SubModuleCategoryDTO(x)).ToArray();
        }

        public SubModuleCategoryDTO[] GetByUser(int userId)
        {
            return this.SubModuleCategoryModel.GetByUser(userId).Select(x => new SubModuleCategoryDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        public SubModuleCategoryDTO Save(SubModuleCategoryDTO resultDto)
        {
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var subModuleItemModel = this.SubModuleCategoryModel;
                var isTransient = resultDto.subModuleCategoryId == 0;
                var subModuleCategory = isTransient ? null : subModuleItemModel.GetOneById(resultDto.subModuleCategoryId).Value;
                subModuleCategory = this.ConvertDto(resultDto, subModuleCategory);
                subModuleItemModel.RegisterSave(subModuleCategory);
                return new SubModuleCategoryDTO(subModuleCategory);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("SubModuleCategory.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategorySaveAllDTO"/>.
        /// </returns>
        public SubModuleCategorySaveAllDTO SaveAll(SubModuleCategoryDTO[] results)
        {
            results = results ?? new SubModuleCategoryDTO[] { };
            var result = new SubModuleCategorySaveAllDTO();
            var faults = new List<string>();
            var created = new List<SubModuleCategory>();
            foreach (var subModuleCategoryDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(subModuleCategoryDTO, out validationResult))
                {
                    var sessionModel = this.SubModuleCategoryModel;
                    var isTransient = subModuleCategoryDTO.subModuleCategoryId == 0;
                    var appletResult = isTransient ? null : sessionModel.GetOneById(subModuleCategoryDTO.subModuleCategoryId).Value;
                    appletResult = this.ConvertDto(subModuleCategoryDTO, appletResult);
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
                result.saved = created.Select(x => new SubModuleCategoryDTO(x)).ToArray();
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
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        public SubModuleCategoryDTO GetById(int id)
        {
            SubModuleCategory subModuleCategory;
            if ((subModuleCategory = this.SubModuleCategoryModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SubModuleCategory.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SubModuleCategoryDTO(subModuleCategory);
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
            SubModuleCategory subModuleCategory;
            var model = this.SubModuleCategoryModel;
            if ((subModuleCategory = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("SubModuleCategory.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(subModuleCategory, true);
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
        /// The <see cref="PagedSubModuleCategoryDTO"/>.
        /// </returns>
        public PagedSubModuleCategoryDTO GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedSubModuleCategoryDTO
            {
                objects = this.SubModuleCategoryModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new SubModuleCategoryDTO(x)).ToArray(),
                totalCount = totalCount
            };
        }

        /// <summary>
        /// The get applet categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        public SubModuleCategoryDTO[] GetAppletCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetAppletCategoriesByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get SN profile categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        public SubModuleCategoryDTO[] GetSNProfileCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetSNProfileCategoriesByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get quiz categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        public SubModuleCategoryDTO[] GetQuizCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetQuizCategoriesByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get test categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        public SubModuleCategoryDTO[] GetTestCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetTestCategoriesByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get survey categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        public SubModuleCategoryDTO[] GetSurveyCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetSurveyCategoriesByUserId(userId).ToArray();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="smc">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategory"/>.
        /// </returns>
        private SubModuleCategory ConvertDto(SubModuleCategoryDTO smc, SubModuleCategory instance)
        {
            instance = instance ?? new SubModuleCategory();
            instance.IsActive = smc.isActive;
            instance.CategoryName = smc.categoryName;
            instance.DateModified = DateTime.Now;
            instance.SubModule = this.SubModuleModel.GetOneById(smc.subModuleId).Value;
            instance.User = this.UserModel.GetOneById(smc.userId).Value;
            instance.ModifiedBy = smc.modifiedBy.HasValue ? this.UserModel.GetOneById(smc.modifiedBy.Value).Value : null;
            return instance;
        }

        #endregion
    }
}