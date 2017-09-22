// ReSharper disable once CheckNamespace
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
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ACSessionService : BaseService, IACSessionService
    {
        #region Properties

        private ACUserModeModel ACUserModeModel => IoC.Resolve<ACUserModeModel>();
        private LanguageModel LanguageModel => IoC.Resolve<LanguageModel>();

        #endregion

        #region Public Methods and Operators

        public ACSessionDTO Save(ACSessionDTO session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            try
            {
                ValidationResult validationResult;
                if (IsValid(session, out validationResult))
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
            catch (Exception ex)
            {
                Logger.Error("ACSession.Save", ex);
                throw;
            }
        }

        #endregion

        #region Methods

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