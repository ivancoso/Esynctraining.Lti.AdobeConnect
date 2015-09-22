// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
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
    public class ACSessionService : BaseService, IACSessionService
    {
        #region Properties

        /// <summary>
        /// Gets the ac user mode model.
        /// </summary>
        private ACUserModeModel ACUserModeModel
        {
            get
            {
                return IoC.Resolve<ACUserModeModel>();
            }
        }

        /// <summary>
        /// Gets the language model.
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
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteById(int id)
        {
            ACSession session;
            if ((session = this.ACSessionModel.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_SESSION,
                    ErrorsTexts.SessionError_Subject,
                    ErrorsTexts.SessionError_NotFound);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            this.ACSessionModel.RegisterDelete(session, true);
            return id;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ACSessionDTO"/>.
        /// </returns>
        public ACSessionDTO GetById(int id)
        {
            ACSession session;
            if ((session = this.ACSessionModel.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_SESSION,
                    ErrorsTexts.SessionError_Subject,
                    ErrorsTexts.SessionError_NotFound);
                this.LogError("ACSession.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new ACSessionDTO(session);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="smiId">
        /// The SMI Id.
        /// </param>
        /// <returns>
        /// The <see cref="ACSessionDTO"/>.
        /// </returns>
        public ACSessionDTO[] GetBySMIId(int smiId)
        {
            var allBySmiId = this.ACSessionModel.GetAllBySmiId(smiId);
            return allBySmiId.ToList().Select(x => new ACSessionDTO(x)).ToArray();
        }

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="session">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ACSessionDTO"/>.
        /// </returns>
        public ACSessionDTO Save(ACSessionDTO session)
        {
            ValidationResult validationResult;
            if (this.IsValid(session, out validationResult))
            {
                ACSessionModel sessionModel = this.ACSessionModel;
                bool isTransient = session.acSessionId == 0;
                ACSession sessionInstance = isTransient ? null : sessionModel.GetOneById(session.acSessionId).Value;
                sessionInstance = this.ConvertDto(session, sessionInstance);
                sessionModel.RegisterSave(sessionInstance, true);
                return new ACSessionDTO(sessionInstance);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("ACSession.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
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
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

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