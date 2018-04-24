// ReSharper disable CheckNamespace

using System;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System.Collections.Generic;
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
    public class SubModuleItemService : BaseService, ISubModuleItemService
    {
        private CompanyEventQuizMappingModel CompanyEventQuizMappingModel => IoC.Resolve<CompanyEventQuizMappingModel>();

        #region Public Methods and Operators

        // ISA: Checked; really is in use
        public SubModuleItemDTO[] GetAll()
        {
            return this.SubModuleItemModel.GetAll().Select(x => new SubModuleItemDTO(x)).ToArray();
        }

        public SubModuleItemDTO Save(SubModuleItemDTO resultDto)
        {
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var subModuleItemModel = this.SubModuleItemModel;
                var isTransient = resultDto.subModuleItemId == 0;
                var subModuleItem = isTransient ? null : subModuleItemModel.GetOneById(resultDto.subModuleItemId).Value;
                subModuleItem = this.Convert(resultDto, subModuleItem, true);
                return new SubModuleItemDTO(subModuleItem);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("SubModuleItem.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public SubModuleItemSaveAllDTO SaveAll(SubModuleItemDTO[] results)
        {
            results = results ?? new SubModuleItemDTO[] { };
            var result = new SubModuleItemSaveAllDTO();
            var faults = new List<string>();
            var created = new List<SubModuleItem>();
            foreach (var appletResultDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(appletResultDTO, out validationResult))
                {
                    var sessionModel = this.SubModuleItemModel;
                    var isTransient = appletResultDTO.subModuleItemId == 0;
                    var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.subModuleItemId).Value;
                    appletResult = this.Convert(appletResultDTO, appletResult);
                    created.Add(appletResult);
                }
                else
                {
                    faults.AddRange(this.UpdateResultToString(validationResult));
                }
            }

            if (created.Any())
            {
                result.saved = created.Select(x => new SubModuleItemDTO(x)).ToArray();
            }

            if (faults.Any())
            {
                result.faults = faults.ToArray();
            }

            return result;
        }

        public SubModuleItemDTO GetById(int id)
        {
            SubModuleItem subModuleItem;
            if ((subModuleItem = this.SubModuleItemModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SubModuleItem.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SubModuleItemDTO(subModuleItem);
        }

        public int DeleteById(int id)
        {
            SubModuleItem moduleItem;
            var model = this.SubModuleItemModel;
            if ((moduleItem = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("SubModuleItem.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            foreach (var quiz in moduleItem.Quizes)
            {
                var mappings = CompanyEventQuizMappingModel.GetAllMappedByQuizId(quiz.Id);
                bool informAboutMapping = false;
                foreach (var mapping in mappings)
                {
                    var acUri = new Uri(mapping.CompanyAcDomain.AcServer);
                    var acProvider = new AdobeConnectProvider(new ConnectionDetails(acUri));
                    var acProxy = new AdobeConnectProxy(acProvider, Logger, acUri);
                    var scoInfoResult = acProxy.GetScoInfo(mapping.AcEventScoId);
                    if (scoInfoResult.Status.Code == StatusCodes.no_data)
                    {
                        CompanyEventQuizMappingModel.RegisterDelete(mapping);
                    }
                    else
                    {
                        informAboutMapping = true;
                    }
                }

                if (informAboutMapping)
                {
                    var msg = "This item cannot be removed as it's mapped with an Event. Please delete the Mapping first.";
                    var error = new Error(
                        Errors.CODE_ERRORTYPE_USER_MESSAGE,
                        msg,
                        msg);
                    throw new FaultException<Error>(error, error.errorMessage);
                }
            }

            model.RegisterDelete(moduleItem, true);
            return id;
        }

        // IS: Checked. is in use.
        public PagedSubModuleItemsDTO GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedSubModuleItemsDTO
            {
                objects =
                    this.SubModuleItemModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new SubModuleItemDTO(x)).ToArray(),
                totalCount = totalCount
            };
        }

        public SubModuleItemDTO[] GetAppletSubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetAppletSubModuleItemsByUserId(userId).ToArray();
        }

        public SubModuleItemDTO[] GetSNProfileSubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetSNProfileSubModuleItemsByUserId(userId).ToArray();
        }

        public SubModuleItemDTO[] GetQuizSubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetQuizSMItemsByUserId(userId).ToArray();
        }

        public SubModuleItemDTO[] GetTestSubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetTestSubModuleItemsByUserId(userId).ToArray();
        }

        public SubModuleItemDTO[] GetSurveySubModuleItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetSurveySubModuleItemsByUserId(userId).ToArray();
        }

        #endregion

    }

}