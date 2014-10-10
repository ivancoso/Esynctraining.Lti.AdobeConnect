// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
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
    public class ACSessionService : BaseService, IACSessionService
    {
        #region Properties

        /// <summary>
        ///     Gets the ac user mode model.
        /// </summary>
        private ACUserModeModel ACUserModeModel
        {
            get
            {
                return IoC.Resolve<ACUserModeModel>();
            }
        }

        /// <summary>
        ///     Gets the language model.
        /// </summary>
        private LanguageModel LanguageModel
        {
            get
            {
                return IoC.Resolve<LanguageModel>();
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
            ACSession session;
            if ((session = this.ACSessionModel.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_SESSION, 
                        ErrorsTexts.SessionError_Subject, 
                        ErrorsTexts.SessionError_NotFound));
            }
            else
            {
                this.ACSessionModel.RegisterDelete(session, true);
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
        public ServiceResponse<ACSessionDTO> GetById(int id)
        {
            var result = new ServiceResponse<ACSessionDTO>();
            ACSession session;
            if ((session = this.ACSessionModel.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_SESSION, 
                        ErrorsTexts.SessionError_Subject, 
                        ErrorsTexts.SessionError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new ACSessionDTO(session);
            }

            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="smiId">
        /// The SMI Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<ACSessionDTO> GetBySMIId(int smiId)
        {
            var result = new ServiceResponse<ACSessionDTO>();

            var allBySmiId = this.ACSessionModel.GetAllBySmiId(smiId);
            result.@objects = allBySmiId.ToList().Select(x => new ACSessionDTO(x)).ToList();

            return result;
        }

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="session">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<ACSessionDTO> Save(ACSessionDTO session)
        {
            var result = new ServiceResponse<ACSessionDTO>();
            ValidationResult validationResult;
            if (this.IsValid(session, out validationResult))
            {
                ACSessionModel sessionModel = this.ACSessionModel;
                bool isTransient = session.acSessionId == 0;
                ACSession sessionInstance = isTransient ? null : sessionModel.GetOneById(session.acSessionId).Value;
                sessionInstance = this.ConvertDto(session, sessionInstance);
                sessionModel.RegisterSave(sessionInstance, true);
                result.@object = new ACSessionDTO(sessionInstance);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="sessionDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="ACSession"/>.
        /// </returns>
        private ACSession ConvertDto(ACSessionDTO sessionDTO, ACSession instance)
        {
            instance = instance ?? new ACSession();
            instance.IncludeAcEmails = sessionDTO.includeACEmails;
            instance.AccountId = sessionDTO.accountId;
            instance.MeetingUrl = sessionDTO.meetingURL;
            instance.DateCreated = sessionDTO.dateCreated == DateTime.MinValue ? DateTime.Now : sessionDTO.dateCreated;
            instance.ScoId = sessionDTO.scoId;
            try
            {
                instance.Status = (ACSessionStatusEnum)sessionDTO.status;
            }
            catch (Exception)
            {
                instance.Status = instance.IsTransient() ? ACSessionStatusEnum.Created : ACSessionStatusEnum.Played;
            }

            instance.Language = this.LanguageModel.GetOneById(sessionDTO.languageId).Value;
            instance.SubModuleItem = this.SubModuleItemModel.GetOneById(sessionDTO.subModuleItemId).Value;
            instance.ACUserMode = this.ACUserModeModel.GetOneById(sessionDTO.acUserModeId).Value;
            instance.User = this.UserModel.GetOneById(sessionDTO.userId).Value;
            return instance;
        }

        #endregion
    }
}