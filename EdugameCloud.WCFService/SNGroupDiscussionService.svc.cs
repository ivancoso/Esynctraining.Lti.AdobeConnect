namespace EdugameCloud.WCFService
{
    using System;
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

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SNGroupDiscussionService : BaseService, ISNGroupDiscussionService
    {
        #region Properties

        /// <summary>
        ///     Gets the SNGroupDiscussion Model.
        /// </summary>
        private SNGroupDiscussionModel SNGroupDiscussionModel
        {
            get
            {
                return IoC.Resolve<SNGroupDiscussionModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

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
            SNGroupDiscussion groupDiscussion;
            var model = this.SNGroupDiscussionModel;
            if ((groupDiscussion = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("SNGroupDiscussion.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(groupDiscussion, true);
            this.UpdateCache();
            return id;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SNGroupDiscussionDTO"/>.
        /// </returns>
        public SNGroupDiscussionDTO GetById(int id)
        {
            SNGroupDiscussion groupDiscussion;
            if ((groupDiscussion = this.SNGroupDiscussionModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_SESSION, ErrorsTexts.EntityGetError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SNGroupDiscussion.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SNGroupDiscussionDTO(groupDiscussion);
        }

        /// <summary>
        /// Get by AC session id.
        /// </summary>
        /// <param name="sessionId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SNGroupDiscussionDTO"/>.
        /// </returns>
        public SNGroupDiscussionDTO GetByAcSessionId(int sessionId)
        {
            SNGroupDiscussion groupDiscussion;
            if ((groupDiscussion = this.SNGroupDiscussionModel.GetOneByACSessionId(sessionId).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_SESSION, ErrorsTexts.EntityGetError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SNGroupDiscussion.GetByAcSessionId", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SNGroupDiscussionDTO(groupDiscussion);
        }

        /// <summary>
        /// Saves discussion.
        /// </summary>
        /// <param name="discussion">
        /// The SN GroupDiscussion DTO.
        /// </param>
        /// <returns>
        /// The <see cref="SNGroupDiscussionDTO"/>.
        /// </returns>
        public SNGroupDiscussionDTO Save(SNGroupDiscussionDTO discussion)
        {
            ValidationResult validationResult;
            if (this.IsValid(discussion, out validationResult))
            {
                var model = this.SNGroupDiscussionModel;
                bool isTransient = discussion.snGroupDiscussionId == 0;
                var instance = isTransient ? null : model.GetOneById(discussion.snGroupDiscussionId).Value;
                instance = this.ConvertDto(discussion, instance);
                model.RegisterSave(instance, true);
                this.UpdateCache();
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<AppletResult>(NotificationType.Update, discussion.companyId, instance.Id);
                return new SNGroupDiscussionDTO(instance);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("SNGroupDiscussion.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The update cache.
        /// </summary>
        private void UpdateCache()
        {
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="groupDiscussion">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="SNGroupDiscussion"/>.
        /// </returns>
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