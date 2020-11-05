using System;
using System.Linq;
using ConnectExtensions.Services.Client;
using Esynctraining.AdobeConnect.Security.Abstractions.DomainValidation;

namespace Esynctraining.AdobeConnect.OwinSecurity.DomainValidation
{
    /// <summary>
    /// TRICK: uses CompanySubscriptionServiceProxy!!
    /// </summary>
    public class Mp4AcDomainValidator : IAcDomainValidator
    {
        public bool IsValid(string companyToken, string acDomain)
        {
            var acDomains = new CompanySubscriptionServiceProxy().GetAdobeConnectDomainsByCompanyToken(companyToken).Result;
            return acDomains.Any(x => x.Equals(acDomain, StringComparison.OrdinalIgnoreCase));
        }

    }

    public class HardCodedDomainValidator : IAcDomainValidator
    {
        private readonly string _validCompanyToken;
        private readonly string _validAcDomain;


        public HardCodedDomainValidator(string validCompanyToken, string validAcDomain)
        {
            if (string.IsNullOrWhiteSpace(validCompanyToken))
                throw new ArgumentException("Non-empty value expected", nameof(validCompanyToken));
            if (string.IsNullOrWhiteSpace(validAcDomain))
                throw new ArgumentException("Non-empty value expected", nameof(validAcDomain));

            _validCompanyToken = validCompanyToken;
            _validAcDomain = validAcDomain;
        }


        public bool IsValid(string companyToken, string acDomain)
        {
            return _validCompanyToken == companyToken && _validAcDomain == acDomain;
        }

    }

    /// <summary>
    /// TRICK: uses PublicLicenseServiceProxy!!
    /// </summary>
    public class ProductAcDomainValidator : IAcDomainValidator
    {
        private readonly int _productId;


        public ProductAcDomainValidator(int productId)
        {
            _productId = productId;
        }


        public bool IsValid(string companyToken, string acDomain)
        {
            var acDomains = new PublicLicenseServiceProxy().GetAdobeConnectDomainsByCompanyToken(companyToken, _productId).Result;
            return acDomains.Any(x => x.Equals(acDomain, StringComparison.OrdinalIgnoreCase));
        }

    }

}
