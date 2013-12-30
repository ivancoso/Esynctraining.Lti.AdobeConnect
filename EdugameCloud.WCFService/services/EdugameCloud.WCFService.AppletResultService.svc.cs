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
    ///     The account service.
    /// </summary>
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
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<AppletResultDTO> GetAll()
        {
            return new ServiceResponse<AppletResultDTO> { objects = this.AppletResultModel.GetAll().Select(x => new AppletResultDTO(x)).ToList() };
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<AppletResultDTO> Save(AppletResultDTO appletResultDTO)
        {
            var result = new ServiceResponse<AppletResultDTO>();
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var sessionModel = this.AppletResultModel;
                var isTransient = appletResultDTO.appletResultId == 0;
                var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.appletResultId).Value;
                appletResult = this.ConvertDto(appletResultDTO, appletResult);
                sessionModel.RegisterSave(appletResult, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<AppletResult>(NotificationType.Update, appletResultDTO.companyId, appletResult.Id);
                result.@object = new AppletResultDTO(appletResult);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTOs">
        /// The applet result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        // ReSharper disable once InconsistentNaming
        public ServiceResponse<AppletResultDTO> SaveAll(List<AppletResultDTO> appletResultDTOs)
        {
            var result = new ServiceResponse<AppletResultDTO>();
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
                    IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<AppletResult>(NotificationType.Update, companyId, 0);
                }

                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new AppletResultDTO(x)).ToList();
            }

            if (faults.Any())
            {
                result.status = created.Any() ? Errors.CODE_RESULTTYPE_PARTIALSUCCESS : Errors.CODE_RESULTTYPE_ERROR;
                result.SetError(new Error(faults.Any(x => x.StartsWith("108")) ? Errors.CODE_ERRORTYPE_INVALID_SESSION : Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.EntityCreationError_Subject, ErrorsTexts.EntityCreation_PartialSuccessMessage, faults));
            }

            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
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
        public ServiceResponse<AppletResultDTO> GetById(int id)
        {
            var result = new ServiceResponse<AppletResultDTO>();
            AppletResult appletResult;
            if ((appletResult = this.AppletResultModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new AppletResultDTO(appletResult);
            }

            return result;
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
            AppletResult appletResult;
            var model = this.AppletResultModel;
            if ((appletResult = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(appletResult, true);
                int companyId = appletResult.With(x => x.AppletItem).With(x => x.SubModuleItem).With(x => x.CreatedBy).With(x => x.Company.Id);
                if (companyId != default(int))
                {
                    IoC.Resolve<RTMPModel>()
                        .NotifyClientsAboutChangesInTable<AppletResult>(
                            NotificationType.Delete,
                            companyId,
                            appletResult.Id);
                }

                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
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
            instance.StartTime = resultDTO.startTime;
            instance.EndTime = resultDTO.endTime;
            instance.IsArchive = resultDTO.isArchive;
            instance.DateCreated = resultDTO.dateCreated == DateTime.MinValue ? DateTime.Now : resultDTO.dateCreated;
            instance.ParticipantName = resultDTO.participantName.With(x => x.Trim());
            instance.AppletItem = this.AppletItemModel.GetOneById(resultDTO.appletItemId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            instance.Email = resultDTO.email.With(x => x.Trim());
            
            return instance;
        }

        #endregion
    }
}