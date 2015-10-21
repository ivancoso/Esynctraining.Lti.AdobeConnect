using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.LmsUserUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ConsumerKeyParameterName = "consumerkey";
            const string neMutexName = "EdugameCloud.Lti.LmsUserUpdater.BackgroundMutexName";
            var parameters = ParseArgumentList(args);
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
                    if (parameters.ContainsKey(ConsumerKeyParameterName))
                    {
                        companies = companies.Where(x => x.ConsumerKey == parameters[ConsumerKeyParameterName]).ToList();
                    }

                    companies = companies.Where(x => x.IsActive && !LicenseExpired(x)).ToList();
                    var groupedByCompany = companies.GroupBy(x => x.LmsProviderId);//.ToDictionary(x => x.Key, y => y.SelectMany(z=>z.LmsCourseMeetings).GroupBy(c => new CourseCompany { CourseId = c.CourseId, LmsCompanyId = c.LmsCompany.Id }));

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
                                    lmsCompany.LmsCourseMeetings.Any(x => x.LmsMeetingType != (int) LmsMeetingType.OfficeHours))
                                {
                                    try
                                    {
                                        syncService.SynchronizeUsers(lmsCompany, syncACUsers: true);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.ErrorFormat(ex, "Unexpected error during execution for LmsCompanyId: {0}.", lmsCompany.Id);
                                    }
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

        private static bool LicenseExpired(LmsCompany lmsCompany)
        {
            var companyModel = IoC.Resolve<CompanyModel>();
            var company = companyModel.GetOneById(lmsCompany.CompanyId).Value;
            return (company == null) || !company.IsActive();
        }

        private static Dictionary<string, string> ParseArgumentList(string[] args)
        {
            var result = new Dictionary<string, string>();
            if (args != null)
            {
                foreach (string argument in args)
                {
                    if (argument.Length > 0)
                    {
                        switch (argument[0])
                        {
                            case '-':
                            case '/':
                                int endIndex = argument.IndexOfAny(new char[] { ':' }, 1);
                                string option = argument.Substring(1, endIndex == -1 ? argument.Length - 1 : endIndex - 1);
                                string optionArgument;
                                if (option.Length + 1 == argument.Length)
                                {
                                    optionArgument = null;
                                }
                                else if (argument.Length > 1 + option.Length && argument[1 + option.Length] == ':')
                                {
                                    optionArgument = argument.Substring(option.Length + 2);
                                }
                                else
                                {
                                    optionArgument = argument.Substring(option.Length + 1);
                                }

                                result.Add(option.ToLowerInvariant(), optionArgument);
                                break;
                        }
                        // skipping other parameter signs
                    }
                }
            }

            return result;
        }
    }

}
