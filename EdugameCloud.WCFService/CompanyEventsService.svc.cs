﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.WCFService.Base;
using EdugameCloud.WCFService.Contracts;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CompanyAcDomainsService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CompanyAcDomainsService.svc or CompanyAcDomainsService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class CompanyEventsService : BaseService, ICompanyEventsService
    {
        /// <summary>
        /// Gets the company model.
        /// </summary>
        private CompanyAcServerModel CompanyAcServerModel
        {
            get { return IoC.Resolve<CompanyAcServerModel>(); }
        }

        private CompanyEventQuizMappingModel CompanyEventQuizMappingModel
        {
            get { return IoC.Resolve<CompanyEventQuizMappingModel>(); }
        }

        private QuizModel QuizModel
        {
            get { return IoC.Resolve<QuizModel>(); }
        }

        private ILogger Logger
        {
            get { return IoC.Resolve<ILogger>(); }
        }

        public CompanyEventDTO[] GetEventsByCompany(int companyId)
        {
            var defaultAcDomain = CompanyAcServerModel.GetAllByCompany(companyId).FirstOrDefault(x => x.IsDefault);
            if (defaultAcDomain == null)
            {
                //WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                return null;
            }

            return GetAllEventsFromAcServer(defaultAcDomain);
        }

        private CompanyEventDTO[] GetAllEventsFromAcServer(CompanyAcServer defaultAcDomain)
        {
            var acUri = new Uri(defaultAcDomain.AcServer);
            var acProvider = new AdobeConnectProvider(new ConnectionDetails(acUri));
            var acProxy = new AdobeConnectProxy(acProvider, Logger, acUri);
            var loginResult = acProxy.Login(new UserCredentials(defaultAcDomain.Username, defaultAcDomain.Password));
            if (!loginResult.Success)
                return null;
            var eventType = acProxy.GetShortcutByType("events");
            var eventContent = acProxy.GetScoExpandedContent(eventType.ScoId);
            if (!eventContent.Success)
                return null;
            var result = new List<CompanyEventDTO>();
            var eventsOnly = eventContent.Values.Where(x => x.Type == "event");
            foreach (var content in eventsOnly)
            {
                result.Add(new CompanyEventDTO()
                {
                    companyId = defaultAcDomain.Company.Id,
                    dateBegin = content.BeginDate,
                    dateEnd = content.EndDate,
                    name = content.Name,
                    desc = content.Description,
                    scoId = content.ScoId,
                    urlPath = content.UrlPath,
                    dateCreated = content.DateCreated,
                    dateModified = content.DateModified,
                    isSeminar = content.IsSeminar
                });
            }
            return result.ToArray();
        }

        public int DeleteById(int id)
        {
            var item = CompanyEventQuizMappingModel.GetOneById(id).Value;
            CompanyEventQuizMappingModel.RegisterDelete(item,true);
            return id;
        }

        public CompanyQuizEventMappingDTO Save(CompanyQuizEventMappingDTO eventQuizMapping)
        {
            var companyAcDomain = CompanyAcServerModel.GetOneById(eventQuizMapping.CompanyAcDomainId).Value;
            var preQuiz = QuizModel.GetOneById(eventQuizMapping.PreQuizId).Value;
            var postQuiz = QuizModel.GetOneById(eventQuizMapping.PostQuizId).Value;
            var companyEventQuizMapping = new CompanyEventQuizMapping()
            {
                Id = eventQuizMapping.EventQuizMappingId,
                CompanyAcDomain = companyAcDomain,
                PreQuiz = preQuiz,
                PostQuiz = postQuiz,
                AcEventScoId = eventQuizMapping.AcEventScoId
            };
            CompanyEventQuizMappingModel.RegisterSave(companyEventQuizMapping, true);
            eventQuizMapping.EventQuizMappingId = companyEventQuizMapping.Id;
            return eventQuizMapping;
        }

        public CompanyEventDTO[] GetEventsByCompanyAcServer(int companyAcServerId)
        {
            var defaultAcDomain = CompanyAcServerModel.GetOneById(companyAcServerId).Value;
            if (defaultAcDomain == null)
            {
                //WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                return null;
            }

            return GetAllEventsFromAcServer(defaultAcDomain);
        }

        public CompanyQuizEventMappingDTO[] GetEventQuizMappings()
        {
            var events = CompanyEventQuizMappingModel.GetAll();
            var result = events.Select(x => new CompanyQuizEventMappingDTO(x)).ToArray();
            return result;
        }

        public CompanyQuizEventMappingDTO[] GetEventQuizMappingsByCompanyId(int companyId)
        {
            var events = CompanyEventQuizMappingModel.GetAllByCompanyId(companyId);
            var result = events.Select(x => new CompanyQuizEventMappingDTO(x)).ToArray();
            return result;
        }

        public CompanyQuizEventMappingDTO[] GetEventQuizMappingsByAcServerId(int acServerId)
        {
            var events = CompanyEventQuizMappingModel.GetAllByAcServerId(acServerId);
            var result = events.Select(x => new CompanyQuizEventMappingDTO(x)).ToArray();
            return result;
        }
    }
}
