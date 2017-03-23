using AutoMapper;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.Api.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace EdugameCloud.Lti.Api.Controllers
{
    [Route("seminars")]
    public class SeminarController : BaseApiController
    {
        private static readonly IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeetingDTO, SeminarDto>()).CreateMapper();

        // TODO: ask if we need it or use base.Cache
        protected readonly ObjectCache _cache = System.Runtime.Caching.MemoryCache.Default;

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();

        public SeminarController(
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger,
            ICache cache
        )
            : base(acAccountService, settings, logger, cache)
        {

        }

        [HttpPost]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public virtual OperationResult Create([FromBody]CreateSeminarDto model)
        {
            if (string.IsNullOrWhiteSpace(model.seminarLicenseId))
                throw new ArgumentException("seminarLicenseId can't be empty", nameof(model.seminarLicenseId));

            //TRICK:
            model.Type = (int)LmsMeetingType.Seminar;

            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var trace = new StringBuilder();
                var fb = new SeminarFolderBuilder(model.seminarLicenseId);

                OperationResult ret = MeetingSetup.SaveMeeting(
                    Session.LmsCompany,
                    this.GetCurrentUserProvider(Session),
                    param,
                    model,
                    trace,
                    fb);

                return TrickForSeminar(ret, model.seminarLicenseId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("Create", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [HttpPost]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public virtual OperationResult Edit([FromBody]EditSeminarDto model)
        {
            if (string.IsNullOrWhiteSpace(model.lmsProviderName))
                throw new ArgumentException("lmsProviderName can't be empty", nameof(model.lmsProviderName));
            if (string.IsNullOrWhiteSpace(model.seminarLicenseId))
                throw new ArgumentException("seminarLicenseId can't be empty", nameof(model.seminarLicenseId));
        
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var trace = new StringBuilder();
                var fb = new SeminarFolderBuilder(model.seminarLicenseId);
                OperationResult ret = MeetingSetup.SaveMeeting(
                    Session.LmsCompany,
                    this.GetCurrentUserProvider(Session),
                    param,
                    model,
                    trace,
                    fb);

                return TrickForSeminar(ret, model.seminarLicenseId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("Edit", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        private IAdobeConnectProxy GetCurrentUserProvider(LmsUserSession session)
        {
            string cacheKey = CachePolicies.Keys.UserAdobeConnectProxy(session.LmsCompany.Id, session.LtiSession.LtiParam.lms_user_id);
            var provider = _cache.Get(cacheKey) as IAdobeConnectProxy;

            if (provider == null)
            {
                string breezeSession = LoginCurrentUser(session);
                var accService = new Esynctraining.AdobeConnect.AdobeConnectAccountService(Logger);
                provider = accService.GetProvider2(new AdobeConnectAccess2(new Uri(session.LmsCompany.AcServer), breezeSession));

                var sessionTimeout = accService.GetAccountDetails(GetAdminProvider()).SessionTimeout - 1; //-1 is to be sure 
                _cache.Set(cacheKey, provider, DateTimeOffset.Now.AddMinutes(sessionTimeout));
            }

            return provider;
        }

        private string LoginCurrentUser(LmsUserSession session)
        {
            LmsCompany lmsCompany = null;
            try
            {
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var lmsUserModel = IoC.Resolve<LmsUserModel>();
                var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    throw new Core.WarningMessageException($"No user with id {param.lms_user_id} found in the database.");
                }

                if (lmsUser.PrincipalId == null)
                {
                    throw new Core.WarningMessageException("User doesn't have account in Adobe Connect.");
                }

                var ac = GetAdminProvider();
                var registeredUser = ac.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo.Principal;

                var MeetingSetup = IoC.Resolve<MeetingSetup>();
                string breezeToken = MeetingSetup.ACLogin(lmsCompany, param, lmsUser, registeredUser, ac);

                return breezeToken;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-LoginCurrentUser", ex);
                throw;
            }
        }

        private OperationResult TrickForSeminar(OperationResult ret, string seminarLicenseId)
        {
            var res1 = ret as OperationResultWithData<MeetingDTO>;
            if (res1 != null)
            {
                var resultDto = mapper.Map<SeminarDto>(res1.Data);
                resultDto.SeminarLicenseId = seminarLicenseId;
                return resultDto.ToSuccessResult();
            }


            var res2 = ret as OperationResultWithData<MeetingAndLmsUsersDTO>;
            if (res2 != null)
            {
                var seminarDto = mapper.Map<SeminarDto>(res2.Data.Meeting);
                var result = new SeminarAndLmsUsersDTO
                {
                    Meeting = seminarDto,
                    LmsUsers = res2.Data.LmsUsers,
                };
                return result.ToSuccessResult();
            }

            return ret;
        }
    }
}
