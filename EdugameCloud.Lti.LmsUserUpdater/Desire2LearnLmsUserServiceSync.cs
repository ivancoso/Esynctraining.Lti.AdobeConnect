using Castle.Core.Logging;
using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Desire2Learn;
using Esynctraining.Core.Providers;

namespace LmsUserUpdater
{
    public class Desire2LearnLmsUserServiceSync : Desire2LearnLmsUserService
    {
        public Desire2LearnLmsUserServiceSync(ILogger logger, LmsUserModel lmsUserModel, IDesire2LearnApiService d2lApiService, ApplicationSettingsProvider settings) : base(logger, lmsUserModel, d2lApiService, settings)
        {
        }

        protected override bool AllowAdminAdditionToCourse
        {
            get
            {
                return false;
            }
        }
    }
}
