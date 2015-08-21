using System;
using System.Collections.Generic;
using System.Diagnostics;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.AdobeConnect.Caching
{
    public sealed class PrincipalCache : IPrincipalCache
    {
        public void RecreatePrincipalCache(IEnumerable<LmsCompany> acDomains)
        {
            RecreatePrincipalCache(new DummyLog(), acDomains);
        }

        public void RecreatePrincipalCache(ILog log, IEnumerable<LmsCompany> acDomains)
        {
            ReBuildCache(log, acDomains);
        }


        private static void ReBuildCache(ILog logger, IEnumerable<LmsCompany> acDomains)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var processor = new PrincipalCacher(logger);

            CacheCleanup.CleanAllPrincipalCache();

            foreach (LmsCompany license in acDomains)
            {
                try
                {
                    var tmp = new Uri(license.AcServer);
                }
                catch
                {
                    continue;
                }

                processor.Process(license);
            }

            sw.Stop();
            logger.WriteLine("=======");
            logger.WriteLine("Total Elapsed. " + sw.Elapsed.ToString());
        }

    }

}
