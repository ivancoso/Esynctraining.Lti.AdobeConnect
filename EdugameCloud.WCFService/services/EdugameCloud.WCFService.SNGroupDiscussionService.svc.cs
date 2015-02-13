// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
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

        /// <summary>
        ///     Gets the AC session model.
        /// </summary>
        private ACSessionModel ACSessionModel
        {
            get
            {
                return IoC.Resolve<ACSessionModel>();
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> DeleteById(int id)
        {
            var result = new ServiceResponse<int>();
            SNGroupDiscussion groupDiscussion;
            var model = this.SNGroupDiscussionModel;
            if ((groupDiscussion = model.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_SESSION, ErrorsTexts.EntityDeleteError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(groupDiscussion, true);
                this.UpdateCache();
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNGroupDiscussionDTO> GetById(int id)
        {
            var result = new ServiceResponse<SNGroupDiscussionDTO>();
            SNGroupDiscussion groupDiscussion;
            if ((groupDiscussion = this.SNGroupDiscussionModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_SESSION, ErrorsTexts.EntityGetError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SNGroupDiscussionDTO(groupDiscussion);
            }

            return result;
        }

        /// <summary>
        /// Get by AC session id.
        /// </summary>
        /// <param name="sessionId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNGroupDiscussionDTO> GetByAcSessionId(int sessionId)
        {
            var result = new ServiceResponse<SNGroupDiscussionDTO>();
            SNGroupDiscussion groupDiscussion;
            if ((groupDiscussion = this.SNGroupDiscussionModel.GetOneByACSessionId(sessionId).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_SESSION, ErrorsTexts.EntityGetError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SNGroupDiscussionDTO(groupDiscussion);
            }

            return result;
        }

        /// <summary>
        /// Saves discussion.
        /// </summary>
        /// <param name="discussion">
        /// The SN GroupDiscussion DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNGroupDiscussionDTO> Save(SNGroupDiscussionDTO discussion)
        {
            var result = new ServiceResponse<SNGroupDiscussionDTO>();
            ValidationResult validationResult;
            if (this.IsValid(discussion, out validationResult))
            {
                var model = this.SNGroupDiscussionModel;
                bool isTransient = discussion.snGroupDiscussionId == 0;
                var instance = isTransient ? null : model.GetOneById(discussion.snGroupDiscussionId).Value;
                instance = this.ConvertDto(discussion, instance);
                model.RegisterSave(instance, true);
                this.UpdateCache();
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<AppletResult>(NotificationType.Update, discussion.companyId, instance.Id);
                result.@object = new SNGroupDiscussionDTO(instance);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
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
            instance.DateCreated = groupDiscussion.dateCreated == DateTime.MinValue ? DateTime.Now : groupDiscussion.dateCreated;
            instance.DateModified = groupDiscussion.dateModified ?? DateTime.Now;
            instance.ACSessionId = this.ACSessionModel.GetOneById(groupDiscussion.acSessionId).Value.With(x => x.Id);
            instance.IsActive = groupDiscussion.isActive;
            return instance;
        }

        #endregion
    }
}