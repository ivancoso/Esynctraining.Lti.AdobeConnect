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
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Utils;
using System.Text;
using System.Xml.Linq;

namespace EdugameCloud.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CompanyAcDomainsService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CompanyAcDomainsService.svc or CompanyAcDomainsService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class CompanyEventsService : BaseService, ICompanyEventsService
    {
        private CompanyAcServerModel CompanyAcServerModel => IoC.Resolve<CompanyAcServerModel>();

        private CompanyEventQuizMappingModel CompanyEventQuizMappingModel => IoC.Resolve<CompanyEventQuizMappingModel>();

        private QuizModel QuizModel => IoC.Resolve<QuizModel>();

        private QuizResultModel QuizResultModel => IoC.Resolve<QuizResultModel>();


        public CompanyEventDTO[] GetEventsByCompany(int companyId)
        {
            var defaultAcDomain = CompanyAcServerModel.GetAllByCompany(companyId).FirstOrDefault(x => x.IsDefault);
            if (defaultAcDomain == null)
            {
                return null;
            }

            return GetAllEventsFromAcServer(defaultAcDomain);
        }

        private CompanyEventDTO[] GetAllEventsFromAcServer(CompanyAcServer defaultAcDomain, List<string> scoIds, bool isShowPastEvents = false)
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

            if (!isShowPastEvents)
            {
                eventsOnly = eventsOnly.Where(x => x.EndDateLocal >= DateTime.Now);
            }

            foreach (var content in eventsOnly)
            {
                if (!scoIds.Any(s => s == content.ScoId))
                    continue;

                result.Add(new CompanyEventDTO
                {
                    companyId = defaultAcDomain.Company.Id,
                    dateBegin = DateTime.SpecifyKind(content.BeginDate, DateTimeKind.Utc),
                    dateEnd = DateTime.SpecifyKind(content.EndDate, DateTimeKind.Utc),
                    name = content.Name,
                    desc = content.Description,
                    scoId = content.ScoId,
                    urlPath = content.UrlPath,
                    dateCreated = DateTime.SpecifyKind(content.DateCreated, DateTimeKind.Utc),
                    dateModified = DateTime.SpecifyKind(content.DateModified, DateTimeKind.Utc),
                    isSeminar = content.IsSeminar,
                    isMappedToQuizzes = CompanyEventQuizMappingModel.GetAllByCompanyId(defaultAcDomain.Company.Id).Any(x => x.AcEventScoId == content.ScoId),
                    meetingUrl = acProxy.GetScoInfo(content.ScoId).ScoInfo?.SourceSco?.UrlPath,
                });

            }
            return result.ToArray();
        }


        private CompanyEventDTO[] GetAllEventsFromAcServer(CompanyAcServer defaultAcDomain, bool isShowPastEvents = false)
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

            if (!isShowPastEvents)
            {
                eventsOnly = eventsOnly.Where(x => x.EndDateLocal >= DateTime.Now);
            }

            foreach (var content in eventsOnly)
            {
                result.Add(new CompanyEventDTO
                {
                    companyId = defaultAcDomain.Company.Id,
                    dateBegin = DateTime.SpecifyKind(content.BeginDate, DateTimeKind.Utc),
                    dateEnd = DateTime.SpecifyKind(content.EndDate, DateTimeKind.Utc),
                    name = content.Name,
                    desc = content.Description,
                    scoId = content.ScoId,
                    urlPath = content.UrlPath,
                    dateCreated = DateTime.SpecifyKind(content.DateCreated, DateTimeKind.Utc),
                    dateModified = DateTime.SpecifyKind(content.DateModified, DateTimeKind.Utc),
                    isSeminar = content.IsSeminar,
                    isMappedToQuizzes = CompanyEventQuizMappingModel.GetAllByCompanyId(defaultAcDomain.Company.Id).Any(x => x.AcEventScoId == content.ScoId),
                    meetingUrl = acProxy.GetScoInfo(content.ScoId).ScoInfo?.SourceSco?.UrlPath,
                });

            }
            return result.ToArray();
        }

        public int DeleteById(int id)
        {
            var item = CompanyEventQuizMappingModel.GetOneById(id).Value;
            var quizResults = QuizResultModel.GetAll(x => x.EventQuizMapping.Id == id);
            if (quizResults.Any())
                return 0;
            CompanyEventQuizMappingModel.RegisterDelete(item, true);
            return id;
        }

        public CompanyQuizEventMappingSaveDTO Save(CompanyQuizEventMappingSaveDTO eventQuizMapping)
        {
            var companyAcDomain = CompanyAcServerModel.GetOneById(eventQuizMapping.companyAcDomainId).Value;
            var preQuiz = QuizModel.GetOneById(eventQuizMapping.preQuizId).Value;
            var postQuiz = QuizModel.GetOneById(eventQuizMapping.postQuizId).Value;
            var companyEventQuizMapping = new CompanyEventQuizMapping
            {
                Id = eventQuizMapping.eventQuizMappingId,
                CompanyAcDomain = companyAcDomain,
                PreQuiz = preQuiz,
                PostQuiz = postQuiz,
                AcEventScoId = eventQuizMapping.acEventScoId,
            };
            CompanyEventQuizMappingModel.RegisterSave(companyEventQuizMapping, true);
            eventQuizMapping.eventQuizMappingId = companyEventQuizMapping.Id;
            return eventQuizMapping;
        }

        public OperationResultDto IsEventValid(CompanyQuizEventMappingSaveDTO eventQuizMapping)
        {
            var defaultAcDomain = CompanyAcServerModel.GetOneById(eventQuizMapping.companyAcDomainId).Value;
            if (defaultAcDomain == null)
            {
                return OperationResultDto.Error("Company was not found");
            }

            var acUri = new Uri(defaultAcDomain.AcServer);
            var acProvider = new AdobeConnectProvider(new ConnectionDetails(acUri));
            var acProxy = new AdobeConnectProxy(acProvider, Logger, acUri);
            var loginResult = acProxy.Login(new UserCredentials(defaultAcDomain.Username, defaultAcDomain.Password));
            if (!loginResult.Success)
                return null;

            var eventRegistrationDetails = acProxy.GetEventRegistrationDetails(eventQuizMapping.acEventScoId);

            string[] requiredFieldNames = new string[3] {"School", "State", "Trainee number or ID"};
            StringBuilder errorMessage = new StringBuilder();

            foreach (var fieldName in requiredFieldNames)
            {
                if (!eventRegistrationDetails.EventFields.Any(ef =>
                    string.Equals(ef.Description, fieldName, StringComparison.OrdinalIgnoreCase)))
                {
                    errorMessage.Append($"{fieldName} field is missed.{Environment.NewLine}");
                }
            }

            var eventInfo = acProxy.GetEventInfo(eventQuizMapping.acEventScoId);

            if (string.IsNullOrEmpty(eventInfo.ScoInfo.Info))
            {
                errorMessage.Append(
                    $"Event info should be set for the event with sco-id {eventQuizMapping.acEventScoId}.{Environment.NewLine}");
            }

            if (string.IsNullOrEmpty(eventInfo.ScoInfo.SpeakerName))
            {
                errorMessage.Append(
                    $"Speaker name should be set for the event with sco-id {eventQuizMapping.acEventScoId}.{Environment.NewLine}");
            }

            if (!string.IsNullOrEmpty(errorMessage.ToString()))
            {
                errorMessage.Append(
                    $"Please fix the event details in Adobe Connect and try again.{Environment.NewLine}");
            }

            return string.IsNullOrEmpty(errorMessage.ToString())
                ? OperationResultDto.Success()
                : OperationResultDto.Error(errorMessage.ToString());
        }

        public CompanyEventDTO[] GetEventsByCompanyAcServer(int companyAcServerId)
        {
            var defaultAcDomain = CompanyAcServerModel.GetOneById(companyAcServerId).Value;
            if (defaultAcDomain == null)
            {
                return null;
            }

            return GetAllEventsFromAcServer(defaultAcDomain);
        }

        public CompanyQuizEventMappingDTO[] GetEventQuizMappings()
        {
            var events = CompanyEventQuizMappingModel.GetAll();
            var result = events.Select(x => new CompanyQuizEventMappingDTO(x, Settings)).ToArray();
            return result;
        }

        public CompanyQuizEventMappingDTO[] GetEventQuizMappingsByCompanyId(int companyId)
        {
            var events = CompanyEventQuizMappingModel.GetAllByCompanyId(companyId);
            var defaultAcDomain = CompanyAcServerModel.GetAllByCompany(companyId).FirstOrDefault(x => x.IsDefault);
            if (defaultAcDomain == null)
            {
                return null;
            }

            var acEvents = GetAllEventsFromAcServer(defaultAcDomain, events.Select(e => e.AcEventScoId).ToList(), true);
            if (acEvents == null)
            {
                return null;
            }

            var result = events.Select(x => new CompanyQuizEventMappingDTO(x, Settings, acEvents.FirstOrDefault(ev => ev.scoId == x.AcEventScoId), CompanyAcServerModel.GetOneById(x.CompanyAcDomain.Id).Value)).ToArray();
            return result;
        }

        public CompanyQuizEventMappingDTO[] GetEventQuizMappingsByAcServerId(int acServerId)
        {
            var events = CompanyEventQuizMappingModel.GetAllByAcServerId(acServerId);
            var defaultAcDomain = CompanyAcServerModel.GetOneById(acServerId).Value;
            if (defaultAcDomain == null)
            {
                return null;
            }
            var acEvents = GetAllEventsFromAcServer(defaultAcDomain, events.Select(e => e.AcEventScoId).ToList(), true);
            var result = events.Select(x => new CompanyQuizEventMappingDTO(x, Settings, acEvents.FirstOrDefault(ev => ev.scoId == x.AcEventScoId))).ToArray();
            return result;
        }

        public CompanyQuizEventMappingDTO GetById(int eventQuizMappingId)
        {
            var @event = CompanyEventQuizMappingModel.GetOneById(eventQuizMappingId).Value;
            return new CompanyQuizEventMappingDTO(@event, Settings);
        }

        public CompanyEventDTO[] GetEventsByCompanyAcServerWithPastEvents(int companyAcServerId)
        {
            var defaultAcDomain = CompanyAcServerModel.GetOneById(companyAcServerId).Value;
            if (defaultAcDomain == null)
            {
                return null;
            }

            return GetAllEventsFromAcServer(defaultAcDomain, true);
        }

        public CompanyQuizEventMappingDTO GetByGuid(Guid id)
        {
            Logger.Info($"event mapping quid is {id}");
            var entity = CompanyEventQuizMappingModel.GetByGuid(id);

            return new CompanyQuizEventMappingDTO(entity, Settings);
        }

    }

}
