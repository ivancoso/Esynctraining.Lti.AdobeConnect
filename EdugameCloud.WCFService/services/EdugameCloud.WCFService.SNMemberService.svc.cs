// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
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
    ///     The SN Group Discussion Message service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SNMemberService : BaseService, ISNMemberService
    {
        #region Properties

        /// <summary>
        /// Gets the SN session member model.
        /// </summary>
        private SNMemberModel SNSessionMemberModel
        {
            get
            {
                return IoC.Resolve<SNMemberModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets all members.
        /// </summary>
        /// <param name="sessionId">
        /// The AC Session Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNMemberDTO> GetAllByACSessionId(int sessionId)
        {
            return new ServiceResponse<SNMemberDTO>
            {
                objects =
                    this.SNSessionMemberModel.GetAllByACSessionId(sessionId)
                        .Select(x => new SNMemberDTO(x))
                        .ToList()
            };
        }

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
            SNMember sessionMember;
            var model = this.SNSessionMemberModel;
            if ((sessionMember = model.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_SESSION, ErrorsTexts.EntityDeleteError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(sessionMember, true);
                this.UpdateCache();
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable(NotificationType.Delete, this.CurrentUser.With(x => x.Company.Id), sessionMember);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        /// Gets one by id.
        /// </summary>
        /// <param name="id">
        /// The Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNMemberDTO> GetById(int id)
        {
            var result = new ServiceResponse<SNMemberDTO>();
            SNMember profile;
            if ((profile = this.SNSessionMemberModel.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_SESSION,
                        ErrorsTexts.EntityGetError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SNMemberDTO(profile);
            }

            return result;
        }

        /// <summary>
        /// Saves session.
        /// </summary>
        /// <param name="sessionMember">
        /// The SN session DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNMemberDTO> Save(SNMemberDTO sessionMember)
        {
            var result = new ServiceResponse<SNMemberDTO>();
            ValidationResult validationResult;
            if (this.IsValid(sessionMember, out validationResult))
            {
                var model = this.SNSessionMemberModel;
                bool isTransient = sessionMember.snMemberId == 0;
                var instance = isTransient ? null : model.GetOneById(sessionMember.snMemberId).Value;
                instance = this.ConvertDto(sessionMember, instance);
                model.RegisterSave(instance, true);
                this.UpdateCache();
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<SNMember>(NotificationType.Update, sessionMember.companyId, instance.Id);
                result.@object = new SNMemberDTO(instance);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNMemberDTO> SaveAll(List<SNMemberDTO> results)
        {
            var result = new ServiceResponse<SNMemberDTO>();
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
                    IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<SNMember>(NotificationType.Update, companyId, 0);
                }

                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new SNMemberDTO(x)).ToList();
            }

            if (faults.Any())
            {
                result.status = created.Any() ? Errors.CODE_RESULTTYPE_PARTIALSUCCESS : Errors.CODE_RESULTTYPE_ERROR;
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.EntityCreationError_Subject, ErrorsTexts.EntityCreation_PartialSuccessMessage, faults));
            }

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
            instance.DateCreated = sessionMember.dateCreated.HasValue ? sessionMember.dateCreated : DateTime.Now;
            instance.IsBlocked = sessionMember.isBlocked;
            instance.ACSessionId = this.ACSessionModel.GetOneById(sessionMember.acSessionId).Value.With(x => x.Id);
            return instance;
        }

        #endregion
    }
}