using System;
using System.Linq;
using System.Runtime.Serialization;
using EdugameCloud.Core.Domain.Entities;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Core.Domain.DTO
{

    [DataContract]
    public class CompanyFlatDTO
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public bool isActive { get; set; }

        [DataMember]
        public bool isActiveTrial { get; set; }

        [DataMember]
        public bool isExpiredTrial { get; set; }

        [DataMember]
        public double dateCreated { get; set; }

        [DataMember]
        public double dateModified { get; set; }

        public static CompanyFlatDTO CreateCompanyFlatDto(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException("company");
            }
            var now = DateTime.Now;
            var license =
                   company.Licenses.ToList().Where(cl => cl.DateStart <= DateTime.Now)
                   .OrderByDescending(cl => (int)cl.LicenseStatus)
                   .ThenByDescending(cl => cl.DateCreated)
                   .FirstOrDefault();

            var isTrial = (license != null) && (license.LicenseStatus == CompanyLicenseStatus.Trial);
            return new CompanyFlatDTO
            {
                id = company.Id,
                name = company.CompanyName,
                isActive = company.Status == CompanyStatus.Active,

                dateCreated = company.DateCreated.ConvertToUnixTimestamp(),
                dateModified = company.DateModified.ConvertToUnixTimestamp(),

                isActiveTrial = isTrial && (license.ExpiryDate >= now),
                isExpiredTrial = isTrial && (license.ExpiryDate < now),
            };
        }

    }

}
