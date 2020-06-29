namespace EdugameCloud.WCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    //using EdugameCloud.Core.RTMP;
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
        private AppletItemModel AppletItemModel => IoC.Resolve<AppletItemModel>();

        #region Public Methods and Operators

        public AppletItemDTO[] GetAll()
        {
            return this.AppletItemModel.GetAll().Select(x => new AppletItemDTO(x)).ToArray();
        }

        public AppletItemDTO[] GetByUser(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            return this.AppletItemModel.GetByUser(userId).Select(x => new AppletItemDTO(x)).ToArray();
        }
        
        public AppletItemDTO Save(AppletItemDTO appletResultDTO)
        {
            if (appletResultDTO == null)
                throw new ArgumentNullException(nameof(appletResultDTO));

            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var sessionModel = this.AppletItemModel;
                var isTransient = appletResultDTO.appletItemId == 0;
                var appletItem = isTransient ? null : sessionModel.GetOneById(appletResultDTO.appletItemId).Value;
                appletItem = this.ConvertDto(appletResultDTO, appletItem);
                sessionModel.RegisterSave(appletItem, true);
                //int companyId = appletItem.With(x => x.SubModuleItem).With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
                //IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<AppletItem>(NotificationType.Update, companyId, appletItem.Id);
                return new AppletItemDTO(appletItem);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("AppletItem.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

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

        public CrosswordDTO[] GetCrosswordsByUserId(int userId)
        {
            return this.AppletItemModel.GetCrosswordsByUserId(userId).ToArray();
        }

        public CrosswordDTO[] GetSharedCrosswordsByUserId(int userId)
        {
            return this.AppletItemModel.GetSharedCrosswordsByUserId(userId).ToArray();
        }

        #endregion

        #region Methods

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