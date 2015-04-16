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
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class AppletResultService : BaseService, IAppletResultService
    {
        #region Properties

        /// <summary>
        /// Gets the ac user mode model.
        /// </summary>
        private AppletResultModel AppletResultModel
        {
            get
            {
                return IoC.Resolve<AppletResultModel>();
            }
        }

        /// <summary>
        /// Gets the AppletItem model.
        /// </summary>
        private AppletItemModel AppletItemModel
        {
            get
            {
                return IoC.Resolve<AppletItemModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All applet results test.
        /// </summary>
        /// <returns>
        ///     The <see cref="AppletResultDTO" />.
        /// </returns>
        public AppletResultDTO[] GetAll()
        {
            return this.AppletResultModel.GetAll().Select(x => new AppletResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="AppletResultDTO"/>.
        /// </returns>
        public AppletResultDTO Save(AppletResultDTO appletResultDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var sessionModel = this.AppletResultModel;
                var isTransient = appletResultDTO.appletResultId == 0;
                var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.appletResultId).Value;
                appletResult = this.ConvertDto(appletResultDTO, appletResult);
                sessionModel.RegisterSave(appletResult, true);
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<AppletResult>(NotificationType.Update, appletResultDTO.companyId, appletResult.Id);
                return new AppletResultDTO(appletResult);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("AppleiResult.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTOs">
        /// The applet result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="AppletResultDTO"/>.
        /// </returns>
        // ReSharper disable once InconsistentNaming
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
                var companyId = appletResultDTOs.FirstOrDefault(x => x.companyId != 0).With(x => x.companyId);
                if (companyId != default(int))
                {
                    IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<AppletResult>(NotificationType.Update, companyId, 0);
                }

                result.saved = created.Select(x => new AppletResultDTO(x)).ToArray();
            }
            else
            {
                result.saved = new AppletResultDTO[] { };
            }

            result.faults = faults.ToArray();

            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="AppletResultDTO"/>.
        /// </returns>
        public AppletResultDTO GetById(int id)
        {
            AppletResult appletResult;
            if ((appletResult = this.AppletResultModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("AppletResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new AppletResultDTO(appletResult);
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
            AppletResult appletResult;
            var model = this.AppletResultModel;
            if ((appletResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("AppletResult.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }
            
            model.RegisterDelete(appletResult, true);
            int companyId = appletResult.With(x => x.AppletItem).With(x => x.SubModuleItem).With(x => x.CreatedBy).With(x => x.Company.Id);
            if (companyId != default(int))
            {
                IoC.Resolve<RealTimeNotificationModel>()
                    .NotifyClientsAboutChangesInTable<AppletResult>(
                        NotificationType.Delete,
                        companyId,
                        appletResult.Id);
            }

            return id;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="resultDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="AppletResult"/>.
        /// </returns>
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