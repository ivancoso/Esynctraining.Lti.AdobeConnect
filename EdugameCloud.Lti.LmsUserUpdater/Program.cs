using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.LmsUserUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ConsumerKeyParameterName = "consumerkey";
            const string ConsumerKeyOutParameterName = "consumerkeyout";
            const string SplitSyncModeParameterName = "splitsyncmode";

            const string neMutexName = "EdugameCloud.Lti.LmsUserUpdater.BackgroundMutexName";
            var parameters = ParseArgumentList(args);

            // prevent two instances from running
            bool created;

            using (Mutex m = new Mutex(true, neMutexName, out created))
            {
                IoCStart.Init();
                ILogger logger = IoC.Resolve<ILogger>();
                dynamic settings = IoC.Resolve<ApplicationSettingsProvider>();

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
                    //var timer = Stopwatch.StartNew();
                    IEnumerable<LmsCompany> companies = lmsCompanyModel.GetEnabledForSynchronization(parameters.ContainsKey(ConsumerKeyParameterName)
                        ? parameters[ConsumerKeyParameterName]
                        : null);
                    //timer.Stop();
                    //logger.Warn($"Retrieve companies elapsed time: {timer.Elapsed.ToString()}");
                    if (parameters.ContainsKey(ConsumerKeyOutParameterName))
                    {
                        var excludeKeys = parameters[ConsumerKeyOutParameterName].Split(new[] { ',', ';' });
                        companies =
                            companies.Where(x => excludeKeys.All(ek => ek != x.ConsumerKey)).ToList();
                    }

                    //SplitSyncMode mode;
                    //if (!Enum.TryParse(settings.SplitSyncMode, out mode))
                    //{
                    //    mode = SplitSyncMode.None;
                    //}

                    SplitSyncMode mode = parameters.ContainsKey(SplitSyncModeParameterName) ? (SplitSyncMode)Enum.Parse(typeof(SplitSyncMode), parameters[SplitSyncModeParameterName]) : SplitSyncMode.None;

                    companies = companies.Where(x => (mode == SplitSyncMode.None || x.Id % 2 == (int)mode)
                        && !LicenseExpired(x)
                        && x.LmsCourseMeetings.Any(y => y.LmsMeetingType != (int)LmsMeetingType.OfficeHours)).ToList();
                    logger.Info($"[Companies to sync] {string.Join(",", companies.Select(x => x.Id))}");
                    var groupedByCompany = companies.GroupBy(x => x.LmsProviderId);//.ToDictionary(x => x.Key, y => y.SelectMany(z=>z.LmsCourseMeetings).GroupBy(c => new CourseCompany { CourseId = c.CourseId, LmsCompanyId = c.LmsCompany.Id }));

                    //todo: Task for each lms if possible
                    foreach (var group in groupedByCompany)
                    {
                        var service = lmsFactory.GetUserService((LmsProviderEnum)group.Key);
                        foreach (var lmsCompany in group)
                        {
                            try
                            {
                                var timer = Stopwatch.StartNew();
                                syncService.SynchronizeUsers(lmsCompany, syncACUsers: true);
                                timer.Stop();
                                logger.Warn($"[Sync time] LicenseId={lmsCompany.Id}, Time={timer.Elapsed.ToString()}");
                            }
                            catch (Exception ex)
                            {
                                logger.ErrorFormat(ex, "Unexpected error during execution for LmsCompanyId: {0}.", lmsCompany.Id);
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
