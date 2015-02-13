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
    public class AppletItemService : BaseService, IAppletItemService
    {
        #region Properties

        /// <summary>
        /// Gets the AppletItem model.
        /// </summary>
        private AppletItemModel AppletItemModel
        {
            get
            {
                return IoC.Resolve<AppletItemModel>();
            }
        }

        /// <summary>
        ///     Gets the SubModuleItemModel model.
        /// </summary>
        private SubModuleItemModel SubModuleItemModel
        {
            get
            {
                return IoC.Resolve<SubModuleItemModel>();
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
        public ServiceResponse<AppletItemDTO> GetAll()
        {
            return new ServiceResponse<AppletItemDTO> { objects = this.AppletItemModel.GetAll().Select(x => new AppletItemDTO(x)).ToList() };
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
        public ServiceResponse<AppletItemDTO> GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<AppletItemDTO>
            {
                objects = this.AppletItemModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new AppletItemDTO(x)).ToList(),
                totalCount = totalCount
            };
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<AppletItemDTO> Save(AppletItemDTO appletResultDTO)
        {
            var result = new ServiceResponse<AppletItemDTO>();
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var sessionModel = this.AppletItemModel;
                var isTransient = appletResultDTO.appletItemId == 0;
                var appletItem = isTransient ? null : sessionModel.GetOneById(appletResultDTO.appletItemId).Value;
                appletItem = this.ConvertDto(appletResultDTO, appletItem);
                sessionModel.RegisterSave(appletItem, true);
                int companyId = appletItem.With(x => x.SubModuleItem).With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<AppletItem>(NotificationType.Update, companyId, appletItem.Id);
                result.@object = new AppletItemDTO(appletItem);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
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
        public ServiceResponse<AppletItemDTO> GetById(int id)
        {
            var result = new ServiceResponse<AppletItemDTO>();
            AppletItem appletResult;
            if ((appletResult = this.AppletItemModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new AppletItemDTO(appletResult);
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
        public ServiceResponse<AppletItemDTO> GetBySMIId(int id)
        {
            var result = new ServiceResponse<AppletItemDTO>();
            AppletItem appletResult;
            if ((appletResult = this.AppletItemModel.GetOneBySMIId(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new AppletItemDTO(appletResult);
            }

            return result;
        }

        /// <summary>
        /// The get crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CrosswordDTO> GetCrosswordsByUserId(int userId)
        {
            return new ServiceResponse<CrosswordDTO> { objects = this.AppletItemModel.GetCrosswordsByUserId(userId).ToList() };
        }

        /// <summary>
        /// Get shared crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CrosswordDTO> GetSharedCrosswordsByUserId(int userId)
        {
            return new ServiceResponse<CrosswordDTO> { objects = this.AppletItemModel.GetSharedCrosswordsByUserId(userId).ToList() };
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert dto.
        /// </summary>
        /// <param name="itemDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="AppletItem"/>.
        /// </returns>
        private AppletItem ConvertDto(AppletItemDTO itemDTO, AppletItem instance)
        {
            instance = instance ?? new AppletItem();
            instance.AppletName = itemDTO.appletName;
            instance.DocumentXML = itemDTO.documentXML;
            instance.SubModuleItem = itemDTO.subModuleItemId.HasValue ? this.SubModuleItemModel.GetOneById(itemDTO.subModuleItemId.Value).Value : null;
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