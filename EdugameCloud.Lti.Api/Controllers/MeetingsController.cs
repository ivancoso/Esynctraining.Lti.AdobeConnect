using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    public class MeetingsController : BaseApiController
    {
        private readonly IAdobeConnectUserService acUserService;

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();
        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>();

        #region Constructors and Destructors

        public MeetingsController(
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            IAdobeConnectUserService acUserService,
            ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
            this.acUserService = acUserService;
        }

        #endregion

        [Route("meetings")]
        [HttpPost]
        [Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual OperationResultWithData<IEnumerable<MeetingDTO>> GetCourseMeetings()
        {
            StringBuilder trace = null;
            var acProvider = this.GetAdminProvider();

            // TODO: implement. will be use be External API only
            //IEnumerable<MeetingDTO> meetings = MeetingSetup.GetMeetings(
            //       LmsCompany,
            //       session.LmsUser,
            //       acProvider,
            //       param,
            //       trace);

            return Enumerable.Empty<MeetingDTO>().ToSuccessResult();
        }

    }

}