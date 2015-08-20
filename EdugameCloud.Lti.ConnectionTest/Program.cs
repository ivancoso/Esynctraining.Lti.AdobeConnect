using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Castle.Core.Logging;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.ConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IoCStart.Init();
            ILogger logger = IoC.Resolve<ILogger>();
            try
            {
                logger.InfoFormat("===== ConnectionTest Starts. DateTime:{0} =====", DateTime.Now);
                LmsFactory lmsFactory = IoC.Resolve<LmsFactory>();
                var lmsCompanyModel = IoC.Resolve<LmsCompanyModel>();
                var syncService = IoC.Resolve<ISynchronizationUserService>();

                var licenses = lmsCompanyModel.GetAll();

                var tester = IoC.Resolve<TestConnectionService>();

                foreach (var lmsCompany in licenses)
                {
                        try
                        {
                           // syncService.SynchronizeUsers(lmsCompany, syncACUsers: true);
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat(ex, "Unexpected error during execution for LmsCompanyId: {0}.", lmsCompany.Id);
                        }
                    
                }

            }
            catch (Exception ex)
            {
                string msg = "Unexpected error during execution ConnectionTest with message: " + ex.Message;
                logger.Error(msg, ex);
            }
            finally
            {
                logger.InfoFormat("===== ConnectionTest stops. DateTime:{0} =====", DateTime.Now);
            }
        }

    }

}
