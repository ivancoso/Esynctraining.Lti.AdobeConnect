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

        [DataMember]
        public bool isDefaultLmsRole { get; set; }


        public LmsCompanyRoleMappingDTO() { }

        public LmsCompanyRoleMappingDTO(string lmsRoleName, int acRole, bool isDefaultLmsRole)
        {
            this.lmsRoleName = lmsRoleName;
            this.acRole = acRole;
            this.isDefaultLmsRole = isDefaultLmsRole;
        }

    }

}
