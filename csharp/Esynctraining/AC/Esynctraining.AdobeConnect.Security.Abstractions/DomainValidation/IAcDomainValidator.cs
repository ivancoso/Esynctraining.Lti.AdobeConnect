namespace Esynctraining.AdobeConnect.Security.Abstractions.DomainValidation
{
    public interface IAcDomainValidator
    {
        bool IsValid(string companyToken, string acDomain);

    }
    
    /// </summary>
    public sealed class NullAcDomainValidator : IAcDomainValidator
    {
        public bool IsValid(string companyToken, string acDomain)
        {
            return true;
        }

    }

}
