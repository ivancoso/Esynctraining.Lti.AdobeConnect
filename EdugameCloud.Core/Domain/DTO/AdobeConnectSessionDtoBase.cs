namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// Base Adobe Connect session DTO.
    /// </summary>
    [DataContract]
    public class AdobeConnectSessionDtoBase
    {
        /// <summary>
        /// Gets or sets the AdobeConnect session id.
        /// </summary>
        [DataMember]
        public int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the AdobeConnect user mode id.
        /// </summary>
        [DataMember]
        public int acUserModeId { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the date created. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [DataMember]
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the participants.
        /// </summary>
        [DataMember]
        public int activeParticipants { get; set; }

        /// <summary>
        /// Gets or sets the participants.
        /// </summary>
        [DataMember]
        public int totalParticipants { get; set; }
        
        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }
        
    }

}