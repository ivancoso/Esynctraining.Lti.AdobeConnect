using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;

namespace Esynctraining.Mp4Service.Tasks.Client
{
    public abstract class ModelBase
    {
        protected readonly IAdobeConnectProxy _ac;
        protected readonly ILogger _logger;


        public ModelBase(ILogger logger, IAdobeConnectAccountService acAccountService, IAdobeConnectAccess access)
        {
            _logger = logger;
            
            _ac = acAccountService.GetProvider(access, true);            
        }
 
    }

}
