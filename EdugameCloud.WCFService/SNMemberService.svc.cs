namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
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

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SNMemberService : BaseService, ISNMemberService
    {
        private SNMemberModel SNSessionMemberModel => IoC.Resolve<SNMemberModel>();

        #region Public Methods and Operators

        /// <summary>
        /// Gets all members.
        /// </summary>
        /// <param name="sessionId">
        /// The AC Session Id.
        /// </param>
        /// <returns>
        /// The <see cref="SNMemberDTO"/>.
        /// </returns>
        public SNMemberDTO[] GetAllByACSessionId(int sessionId)
        {
            return this.SNSessionMemberModel.GetAllByACSessionId(sessionId).Select(x => new SNMemberDTO(x)).ToArray();
        }

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
            SNMember sessionMember;
            var model = this.SNSessionMemberModel;
            if ((sessionMember = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("SNMember.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(sessionMember, true);
            this.UpdateCache();
            IoC.Resolve<RealTimeNotificationModel>()
                .NotifyClientsAboutChangesInTable(
                    NotificationType.Delete,
                    this.CurrentUser.With(x => x.Company.Id),
                    sessionMember);
            return id;
        }

        /// <summary>
        /// Gets one by id.
        /// </summary>
        /// <param name="id">
        /// The Id.
        /// </param>
        /// <returns>
        /// The <see cref="SNMemberDTO"/>.
        /// </returns>
        public SNMemberDTO GetById(int id)
        {
            SNMember profile;
            if ((profile = this.SNSessionMemberModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SNMember.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SNMemberDTO(profile);
        }

        /// <summary>
        /// Saves session.
        /// </summary>
        /// <param name="sessionMember">
        /// The SN session DTO.
        /// </param>
        /// <returns>
        /// The <see cref="SNMemberDTO"/>.
        /// </returns>
        public SNMemberDTO Save(SNMemberDTO sessionMember)
        {
            ValidationResult validationResult;
            if (this.IsValid(sessionMember, out validationResult))
            {
                var model = this.SNSessionMemberModel;
                bool isTransient = sessionMember.snMemberId == 0;
                var instance = isTransient ? null : model.GetOneById(sessionMember.snMemberId).Value;
                instance = this.ConvertDto(sessionMember, instance);
                model.RegisterSave(instance, true);
                this.UpdateCache();
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<SNMember>(NotificationType.Update, sessionMember.companyId, instance.Id);
                return new SNMemberDTO(instance);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("SNMember.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="SNMemberSaveAllDTO"/>.
        /// </returns>
        public SNMemberSaveAllDTO SaveAll(SNMemberDTO[] results)
        {
            results = results ?? new SNMemberDTO[] { };
            var result = new SNMemberSaveAllDTO();
            var faults = new List<string>();
            var created = new List<SNMember>();
            foreach (var appletResultDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(appletResultDTO, out validationResult))
                {
                    var sessionModel = this.SNSessionMemberModel;
                    var isTransient = appletResultDTO.snMemberId == 0;
                    var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.snMemberId).Value;
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
                var companyId = results.FirstOrDefault(x => x.companyId != 0).With(x => x.companyId);
                if (companyId != default(int))
                {
                    IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<SNMember>(NotificationType.Update, companyId, 0);
                }

                result.saved = created.Select(x => new SNMemberDTO(x)).ToArray();
            }

            if (faults.Any())
            {
                result.faults = faults.ToArray();
            }

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
        /// <param name="sessionMember">
        /// The message.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="SNMember"/>.
        /// </returns>
        private SNMember ConvertDto(SNMemberDTO sessionMember, SNMember instance)
        {
            instance = instance ?? new SNMember();
            instance.ParticipantProfile = sessionMember.participantProfile;
            instance.Participant = sessionMember.participant;
            instance.DateCreated = DateTime.Now;
            instance.IsBlocked = sessionMember.isBlocked;
            instance.ACSessionId = this.ACSessionModel.GetOneById(sessionMember.acSessionId).Value.With(x => x.Id);
            return instance;
        }

        #endregion
    }
}