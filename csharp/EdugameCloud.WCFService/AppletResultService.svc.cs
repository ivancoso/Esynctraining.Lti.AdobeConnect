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
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using FluentValidation.Results;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class AppletResultService : BaseService, IAppletResultService
    {
        private AppletResultModel AppletResultModel => IoC.Resolve<AppletResultModel>();

        private AppletItemModel AppletItemModel => IoC.Resolve<AppletItemModel>();

        #region Public Methods and Operators
        
        public AppletResultSaveAllDTO SaveAll(AppletResultDTO[] appletResultDTOs)
        {
            appletResultDTOs = appletResultDTOs ?? new AppletResultDTO[] { };
            var result = new AppletResultSaveAllDTO();
            var faults = new List<string>();
            var created = new List<AppletResult>();
            foreach (var appletResultDTO in appletResultDTOs)
            {
                ValidationResult validationResult;
                if (this.IsValid(appletResultDTO, out validationResult))
                {
                    var sessionModel = this.AppletResultModel;
                    var isTransient = appletResultDTO.appletResultId == 0;
                    var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.appletResultId).Value;
                    appletResult = this.ConvertDto(appletResultDTO, appletResult);
                    sessionModel.RegisterSave(appletResult, true);
                    created.Add(appletResult);
                }
                else
                {
                    faults.AddRange(this.UpdateResultToString(validationResult));
                }
            }

            if (created.Any())
            {
                //var companyId = appletResultDTOs.FirstOrDefault(x => x.companyId != 0).With(x => x.companyId);
                //if (companyId != default(int))
                //{
                //    IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<AppletResult>(NotificationType.Update, companyId, 0);
                //}

                result.saved = created.Select(x => new AppletResultDTO(x)).ToArray();
            }
            else
            {
                result.saved = new AppletResultDTO[] { };
            }

            result.faults = faults.ToArray();

            return result;
        }
        
        #endregion

        #region Methods

        private AppletResult ConvertDto(AppletResultDTO resultDTO, AppletResult instance)
        {
            instance = instance ?? new AppletResult();
            instance.Score = resultDTO.score;
            instance.StartTime = resultDTO.startTime.ConvertFromUnixTimeStamp();
            instance.EndTime = resultDTO.endTime.ConvertFromUnixTimeStamp();
            instance.IsArchive = resultDTO.isArchive;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.ParticipantName = resultDTO.participantName.With(x => x.Trim());
            instance.AppletItem = this.AppletItemModel.GetOneById(resultDTO.appletItemId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            instance.Email = resultDTO.email.With(x => x.Trim());
            
            return instance;
        }

        #endregion

    }

}