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

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SubModuleCategoryService : BaseService, ISubModuleCategoryService
    {
        #region Properties

        private SubModuleModel SubModuleModel => IoC.Resolve<SubModuleModel>();

        private SubModuleCategoryModel SubModuleCategoryModel => IoC.Resolve<SubModuleCategoryModel>();

        #endregion

        #region Public Methods and Operators

        public SubModuleCategoryDTO[] GetAll()
        {
            return this.SubModuleCategoryModel.GetAll().Select(x => new SubModuleCategoryDTO(x)).ToArray();
        }

        public SubModuleCategoryDTO[] GetByUser(int userId)
        {
            return this.SubModuleCategoryModel.GetByUser(userId).Select(x => new SubModuleCategoryDTO(x)).ToArray();
        }

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

        public SubModuleCategoryDTO[] GetAppletCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetAppletCategoriesByUserId(userId).ToArray();
        }

        public SubModuleCategoryDTO[] GetSNProfileCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetSNProfileCategoriesByUserId(userId).ToArray();
        }

        public SubModuleCategoryDTO[] GetQuizCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetQuizCategoriesByUserId(userId).ToArray();
        }

        public SubModuleCategoryDTO[] GetTestCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetTestCategoriesByUserId(userId).ToArray();
        }

        public SubModuleCategoryDTO[] GetSurveyCategoriesByUserId(int userId)
        {
            return this.SubModuleCategoryModel.GetSurveyCategoriesByUserId(userId).ToArray();
        }

        #endregion

        #region Methods

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