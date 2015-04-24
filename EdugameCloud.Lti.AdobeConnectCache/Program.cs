using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.AdobeConnectCache
{
    public static class Program
    {
        public static int Main()
        {
            const string neMutexName = "EdugameCloud.Lti.AdobeConnectCache.BackgroundMutexName";

            // prevent two instances from running
            bool created;

            using (Mutex m = new Mutex(true, neMutexName, out created))
            {
                IoCStart.Init();

                ILog logger = IoC.Resolve<ILog>();
                if (!created)
                {
                    logger.WriteLine("The application could not be run because another instance was already running.");
                    return 1;
                }

                try
                {
                    logger.WriteLine("=====AdobeConnectCache Engine Starts=====");

                    List<LmsCompany> domains = DatabaseAccessor.GetAllLicences();

                    WriteAcDomainInfo(domains, logger);

                    Stopwatch sw = Stopwatch.StartNew();

                    var processor = new AdobeConnectPrincipalCacher(logger);

                    foreach (LmsCompany license in domains)
                    {
                        try
                        {
                            var tmp = new Uri(license.AcServer);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        processor.Process(license);
                    }

                    sw.Stop();
                    logger.WriteLine("=======");
                    logger.WriteLine("Total Elapsed. " + sw.Elapsed.ToString());
                }
                catch (Exception ex)
                {
                    string msg = "Unexpected error during execution with message: " + ex.Message;
                    logger.WriteLine(msg);
                    logger.WriteLine(ex);
                    return 2;
                }
                finally
                {
                    logger.WriteLine("=====AdobeConnectCache Engine stops=====");
                    // TODO: remove for PROD
                    Console.ReadLine();
                }
            }

            return 0;
        }

        private static void WriteAcDomainInfo(List<LmsCompany> domains, ILog logger)
        {
            logger.WriteLine("Invalid AC Urls: ");
            foreach (var lmsLicence in domains)
            {
                try
                {
                    var tmp = new Uri(lmsLicence.AcServer);
                }
                catch (Exception ex)
                {
                    logger.WriteLine("     " + lmsLicence.AcServer);
                }
            }

            logger.WriteLine("Valid AC Urls: ");
            foreach (var lmsLicence in domains)
            {
                try
                {
                    var tmp = new Uri(lmsLicence.AcServer);
                    logger.WriteLine("     " + lmsLicence.AcServer);
                }
                catch (Exception ex)
                {
                }
            }

            logger.WriteLine("=======");
        }

    }

}
