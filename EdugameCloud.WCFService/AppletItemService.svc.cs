namespace EdugameCloud.WCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

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

        #endregion

        #region Public Methods and Operators

        public AppletItemDTO[] GetAll()
        {
            return this.AppletItemModel.GetAll().Select(x => new AppletItemDTO(x)).ToArray();
        }

        public AppletItemDTO[] GetByUser(int userId)
        {
            return this.AppletItemModel.GetByUser(userId).Select(x => new AppletItemDTO(x)).ToArray();
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
        /// The <see cref="PagedAppletItemsDTO"/>.
        /// </returns>
        public PagedAppletItemsDTO GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedAppletItemsDTO
            {
                objects = this.AppletItemModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new AppletItemDTO(x)).ToArray(),
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
        /// The <see cref="AppletItemDTO"/>.
        /// </returns>
        public AppletItemDTO Save(AppletItemDTO appletResultDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var sessionModel = this.AppletItemModel;
                var isTransient = appletResultDTO.appletItemId == 0;
                var appletItem = isTransient ? null : sessionModel.GetOneById(appletResultDTO.appletItemId).Value;
                appletItem = this.ConvertDto(appletResultDTO, appletItem);
                sessionModel.RegisterSave(appletItem, true);
                int companyId = appletItem.With(x => x.SubModuleItem).With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<AppletItem>(NotificationType.Update, companyId, appletItem.Id);
                return new AppletItemDTO(appletItem);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("AppletItem.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="AppletItemDTO"/>.
        /// </returns>
        public AppletItemDTO GetById(int id)
        {
            AppletItem appletResult;
            if ((appletResult = this.AppletItemModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("AppletItem.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new AppletItemDTO(appletResult);
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="AppletItemDTO"/>.
        /// </returns>
        public AppletItemDTO GetBySMIId(int id)
        {
            AppletItem appletResult;
            if ((appletResult = this.AppletItemModel.GetOneBySMIId(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("AppletItem.GetBySMIId", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new AppletItemDTO(appletResult);
        }

        /// <summary>
        /// The get crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="CrosswordDTO"/>.
        /// </returns>
        public CrosswordDTO[] GetCrosswordsByUserId(int userId)
        {
            return this.AppletItemModel.GetCrosswordsByUserId(userId).ToArray();
        }

        /// <summary>
        /// Get shared crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CrosswordDTO"/>.
        /// </returns>
        public CrosswordDTO[] GetSharedCrosswordsByUserId(int userId)
        {
            return this.AppletItemModel.GetSharedCrosswordsByUserId(userId).ToArray();
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