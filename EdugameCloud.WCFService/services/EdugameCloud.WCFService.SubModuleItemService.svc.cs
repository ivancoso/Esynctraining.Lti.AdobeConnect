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
    public class SubModuleItemService : BaseService, ISubModuleItemService
    {
        #region Properties

        /// <summary>
        /// Gets the quiz model.
        /// </summary>
        private QuizModel QuizModel
        {
            get
            {
                return IoC.Resolve<QuizModel>();
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
                var quizResult = isTransient ? null : subModuleItemModel.GetOneById(resultDto.subModuleItemId).Value;
                quizResult = this.ConvertDto(resultDto, quizResult);
                subModuleItemModel.RegisterSave(quizResult);
                result.@object = new SubModuleItemDTO(quizResult);
                return result;
            }

            result = (ServiceResponse<SubModuleItemDTO>)this.UpdateResult(result, validationResult);
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
                    appletResult = this.ConvertDto(appletResultDTO, appletResult);
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
        public ServiceResponse<SubModuleItemDTOFromStoredProcedureDTO> GetQuizSubModuleItemsByUserId(int userId)
        {
            return new ServiceResponse<SubModuleItemDTOFromStoredProcedureDTO>
            {
                objects = this.QuizModel.GetQuizSMItemsByUserId(userId).ToList()
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

        #region Methods

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

        #endregion
    }
}