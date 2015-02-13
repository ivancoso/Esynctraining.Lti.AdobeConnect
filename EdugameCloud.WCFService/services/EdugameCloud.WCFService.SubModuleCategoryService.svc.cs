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
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<SubModuleCategoryDTO> GetAll()
        {
            return new ServiceResponse<SubModuleCategoryDTO> { objects = this.SubModuleCategoryModel.GetAll().Select(x => new SubModuleCategoryDTO(x)).ToList() };
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
        public ServiceResponse<SubModuleCategoryDTO> Save(SubModuleCategoryDTO resultDto)
        {
            var result = new ServiceResponse<SubModuleCategoryDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var subModuleItemModel = this.SubModuleCategoryModel;
                var isTransient = resultDto.subModuleCategoryId == 0;
                var subModuleCategory = isTransient ? null : subModuleItemModel.GetOneById(resultDto.subModuleCategoryId).Value;
                subModuleCategory = this.ConvertDto(resultDto, subModuleCategory);
                subModuleItemModel.RegisterSave(subModuleCategory);
                result.@object = new SubModuleCategoryDTO(subModuleCategory);
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
        public ServiceResponse<SubModuleCategoryDTO> SaveAll(List<SubModuleCategoryDTO> results)
        {
            var result = new ServiceResponse<SubModuleCategoryDTO>();
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
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new SubModuleCategoryDTO(x)).ToList();
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
        public ServiceResponse<SubModuleCategoryDTO> GetById(int id)
        {
            var result = new ServiceResponse<SubModuleCategoryDTO>();
            SubModuleCategory subModuleCategory;
            if ((subModuleCategory = this.SubModuleCategoryModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SubModuleCategoryDTO(subModuleCategory);
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
            SubModuleCategory subModuleCategory;
            var model = this.SubModuleCategoryModel;
            if ((subModuleCategory = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(subModuleCategory, true);
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
        public ServiceResponse<SubModuleCategoryDTO> GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<SubModuleCategoryDTO>
            {
                objects = this.SubModuleCategoryModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new SubModuleCategoryDTO(x)).ToList(),
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleCategoryDTO> GetAppletCategoriesByUserId(int userId)
        {
            return new ServiceResponse<SubModuleCategoryDTO>
            {
                objects = this.SubModuleCategoryModel.GetAppletCategoriesByUserId(userId).ToList()
            };
        }

        /// <summary>
        /// The get SN profile categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleCategoryDTO> GetSNProfileCategoriesByUserId(int userId)
        {
            return new ServiceResponse<SubModuleCategoryDTO>
            {
                objects = this.SubModuleCategoryModel.GetSNProfileCategoriesByUserId(userId).ToList()
            };
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
        public ServiceResponse<SubModuleCategoryDTO> GetQuizCategoriesByUserId(int userId)
        {
            return new ServiceResponse<SubModuleCategoryDTO>
            {
                objects = this.SubModuleCategoryModel.GetQuizCategoriesByUserId(userId).ToList()
            };
        }

        /// <summary>
        /// The get test categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleCategoryDTO> GetTestCategoriesByUserId(int userId)
        {
            return new ServiceResponse<SubModuleCategoryDTO>
            {
                objects = this.SubModuleCategoryModel.GetTestCategoriesByUserId(userId).ToList()
            };
        }

        /// <summary>
        /// The get survey categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleCategoryDTO> GetSurveyCategoriesByUserId(int userId)
        {
            return new ServiceResponse<SubModuleCategoryDTO>
            {
                objects = this.SubModuleCategoryModel.GetSurveyCategoriesByUserId(userId).ToList()
            };
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
            instance.DateModified = smc.dateModified == DateTime.MinValue ? DateTime.Now : smc.dateModified;
            instance.SubModule = this.SubModuleModel.GetOneById(smc.subModuleId).Value;
            instance.User = this.UserModel.GetOneById(smc.userId).Value;
            instance.ModifiedBy = smc.modifiedBy.HasValue ? this.UserModel.GetOneById(smc.modifiedBy.Value).Value : null;
            return instance;
        }

        #endregion
    }
}