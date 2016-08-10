namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Lti.Domain.Entities;

    /// <summary>
    /// The LMS provider DTO.
    /// </summary>
    [DataContract]
    public class LmsProviderDTO
    {
        #region Constructors
        
        public LmsProviderDTO(LmsProvider p)
        {
            if (p == null)
                throw new ArgumentNullException(nameof(p));

            this.lmsProviderId = p.Id;
            this.lmsProviderName = p.LmsProviderName;
            this.shortName = p.ShortName;
        }

        #endregion

        #region Public Properties

        [DataMember]
        public int lmsProviderId { get; set; }

        [DataMember]
        public string lmsProviderName { get; set; }

        [DataMember]
        public string nameWithoutSpaces { get; set; }

        [DataMember]
        public string shortName { get; set; }

        [DataMember]
        public string configUrl { get; set; }

        [DataMember]
        public string instructionsUrl { get; set; }

        [DataMember]
        public LmsCompanyRoleMappingDTO[] defaultRoleMapping { get; set; }

        #endregion

    }

}
