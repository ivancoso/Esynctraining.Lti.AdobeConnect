namespace EdugameCloud.WCFService
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SNGroupDiscussionService : BaseService, ISNGroupDiscussionService
    {
        private SNGroupDiscussionModel SNGroupDiscussionModel => IoC.Resolve<SNGroupDiscussionModel>();

        #region Public Methods and Operators

        public SNGroupDiscussionDTO Save(SNGroupDiscussionDTO discussion)
        {
            if (discussion == null)
                throw new ArgumentNullException(nameof(discussion));

            ValidationResult validationResult;
            if (IsValid(discussion, out validationResult))
            {
                var model = this.SNGroupDiscussionModel;
                bool isTransient = discussion.snGroupDiscussionId == 0;
                var instance = isTransient ? null : model.GetOneById(discussion.snGroupDiscussionId).Value;
                instance = this.ConvertDto(discussion, instance);
                model.RegisterSave(instance, true);
                this.UpdateCache();
                //IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<AppletResult>(NotificationType.Update, discussion.companyId, instance.Id);
                return new SNGroupDiscussionDTO(instance);
            }

            var error = this.GenerateValidationError(validationResult);
            LogError("SNGroupDiscussion.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        #endregion

        #region Methods

        private void UpdateCache()
        {
        }

        private SNGroupDiscussion ConvertDto(SNGroupDiscussionDTO groupDiscussion, SNGroupDiscussion instance)
        {
            instance = instance ?? new SNGroupDiscussion();
            instance.GroupDiscussionTitle = groupDiscussion.groupDiscussionTitle;
            instance.GroupDiscussionData = groupDiscussion.groupDiscussionData;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            instance.ACSessionId = this.ACSessionModel.GetOneById(groupDiscussion.acSessionId).Value.With(x => x.Id);
            instance.IsActive = groupDiscussion.isActive;
            return instance;
        }

        #endregion
    }

}