using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.AdobeConnectCache
{
    internal static class DatabaseAccessor
    {
        public static List<LmsCompany> GetAllLicences()
        {
            return IoC.Resolve<LmsCompanyModel>().GetAll().ToList();
        }

    }

}
