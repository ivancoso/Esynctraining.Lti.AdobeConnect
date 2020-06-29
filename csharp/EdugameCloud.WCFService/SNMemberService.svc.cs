using System.Web;
using Esynctraining.Core.Domain.Entities;

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
    //using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SNMemberService : BaseService, ISNMemberService
    {
        private SNMemberModel SNSessionMemberModel => IoC.Resolve<SNMemberModel>();

        #region Public Methods and Operators

        public SNMemberSaveAllDTO SaveAll(SNMemberDTO[] results)
        {
            results = results ?? new SNMemberDTO[0];

            var result = new SNMemberSaveAllDTO();
            var faults = new List<string>();
            var created = new List<SNMember>();

            var sessionModel = SNSessionMemberModel;
            ACSessionModel acSessionModel = ACSessionModel;

            foreach (var appletResultDTO in results)
            {
                ValidationResult validationResult;
                var isValidDto = IsValid(appletResultDTO, out validationResult);
                var profileModel = new VCFProfileDTO { xmlProfile = appletResultDTO.participantProfile };
                ValidationResult profileValidationResult;
                var isValidProfile = IsValid(profileModel, out profileValidationResult);
                if (isValidDto && isValidProfile)
                {
                    var isTransient = appletResultDTO.snMemberId == 0;
                    var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.snMemberId).Value;
                    appletResult = ConvertDto(appletResultDTO, appletResult, acSessionModel);
                    sessionModel.RegisterSave(appletResult);
                    created.Add(appletResult);
                }
                else
                {
                    if (!isValidDto)
                    {
                        faults.AddRange(UpdateResultToString(validationResult));
                    }
                    if (!isValidProfile)
                    {
                        faults.AddRange(UpdateResultToString(profileValidationResult));
                    }
                }
            }

            if (created.Any())
            {
                //var companyId = results.FirstOrDefault(x => x.companyId != 0).With(x => x.companyId);
                //if (companyId != default(int))
                //{
                //    IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<SNMember>(NotificationType.Update, companyId, 0);
                //}

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

        private static SNMember ConvertDto(SNMemberDTO sessionMember, SNMember instance, ACSessionModel acSessionModel)
        {
            instance = instance ?? new SNMember();
            instance.ParticipantProfile = sessionMember.participantProfile;
            instance.Participant = sessionMember.participant;
            instance.DateCreated = DateTime.Now;
            instance.IsBlocked = sessionMember.isBlocked;
            instance.ACSessionId = acSessionModel.GetOneById(sessionMember.acSessionId).Value.With(x => x.Id);
            return instance;
        }

        #endregion

    }

}