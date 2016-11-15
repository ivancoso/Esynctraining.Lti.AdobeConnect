using System;
using System.Linq;
using ConnectExtensions.Services.Client;

namespace Esynctraining.AdobeConnect.OwinSecurity.DomainValidation
{
    public interface IAcDomainValidator
    {
        bool IsValid(string companyToken, string acDomain);

    }

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

    /// <summary>
    /// Returns true.
    /// </summary>
    public sealed class NullAcDomainValidator : IAcDomainValidator
    {
        public bool IsValid(string companyToken, string acDomain)
        {
            return true;
        }

    }

}
