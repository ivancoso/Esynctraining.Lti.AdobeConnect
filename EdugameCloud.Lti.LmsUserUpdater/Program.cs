using System;
using System.Linq;
using System.Threading;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.LmsUserUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            const string neMutexName = "EdugameCloud.Lti.LmsUserUpdater.BackgroundMutexName";

            // prevent two instances from running
            bool created;

            using (Mutex m = new Mutex(true, neMutexName, out created))
            {
                IoCStart.Init();
                ILogger logger = IoC.Resolve<ILogger>();

                if (!created)
                {
                    logger.Info("The application EdugameCloud.Lti.LmsUserUpdater could not be run because another instance was already running.");
                }

                try
                {
                    logger.InfoFormat("===== Update Lms Users Engine Starts. DateTime:{0} =====", DateTime.Now);
                    LmsFactory lmsFactory = IoC.Resolve<LmsFactory>();
                    var lmsCompanyModel = IoC.Resolve<LmsCompanyModel>();
                    var syncService = IoC.Resolve<ISynchronizationUserService>();

                    var companies = lmsCompanyModel.GetEnabledForSynchronization();
                    var groupedByCompany = companies.GroupBy(x => x.LmsProvider.Id);//.ToDictionary(x => x.Key, y => y.SelectMany(z=>z.LmsCourseMeetings).GroupBy(c => new CourseCompany { CourseId = c.CourseId, LmsCompanyId = c.LmsCompany.Id }));

                    //todo: Task for each lms if possible
                    foreach (var group in groupedByCompany)
                    {
                        var service = lmsFactory.GetUserService((LmsProviderEnum)group.Key);
                        if (service != null)
                        {
                            foreach (var lmsCompany in group)
                            {
                                if (lmsCompany.UseSynchronizedUsers &&
                                    service.CanRetrieveUsersFromApiForCompany(lmsCompany)
                                    && lmsCompany.LmsCourseMeetings != null &&
                                    lmsCompany.LmsCourseMeetings.Any(
                                        x => x.LmsMeetingType != (int) LmsMeetingType.OfficeHours))
                                {
                                    syncService.SynchronizeUsers(lmsCompany);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = "Unexpected error during execution LmsUserUpdater with message: " + ex.Message;
                    logger.Error(msg, ex);
                }
                finally
                {
                    logger.InfoFormat("===== Update Lms Users Engine stops. DateTime:{0} =====", DateTime.Now);
                }
            }
        }
    }
}
