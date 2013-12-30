namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The SN reporting data DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(SNMemberDTO))]
    [KnownType(typeof(SNGroupDiscussionDTO))]
    public class SNReportingDataDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the discussion.
        /// </summary>
        [DataMember]
        public SNGroupDiscussionDTO discussion { get; set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        [DataMember]
        public List<SNMemberDTO> members { get; set; }

        #endregion
    }
}