namespace EdugameCloud.WCFService.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;

    [DataContract]
    [KnownType(typeof(UserDTO))]
    [KnownType(typeof(AddressDTO))]
    [KnownType(typeof(CompanyLicenseDTO))]
    [KnownType(typeof(CompanyLmsDTO))]
    public class CompanyLmsOperationDTO
    {
        [DataMember]
        public CompanyLmsDTO companyLmsVO { get; set; }

        [DataMember]
        public string message { get; set; }

    }

}