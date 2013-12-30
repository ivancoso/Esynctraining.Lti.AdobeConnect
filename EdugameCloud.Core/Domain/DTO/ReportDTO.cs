namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The report DTO.
    /// </summary>
    [DataContract]
    public class ReportDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember] 
        public int? type { get; set; }

        /// <summary>
        /// Gets or sets the subModuleItemId.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string name { get; set; }

        #endregion
    }
}