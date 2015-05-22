namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class LmsCompanyRoleMappingDTO
    {
        [DataMember]
        public string lmsRoleName { get; set; }

        [DataMember]
        public int acRole { get; set; }


        public LmsCompanyRoleMappingDTO() { }

        public LmsCompanyRoleMappingDTO(string lmsRoleName, int acRole)
        {
            this.lmsRoleName = lmsRoleName;
            this.acRole = acRole;
        }

    }

}
