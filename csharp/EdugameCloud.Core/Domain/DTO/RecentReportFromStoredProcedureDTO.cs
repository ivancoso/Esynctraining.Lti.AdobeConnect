namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The recent report DTO.
    /// </summary>
    [DataContract]
    public class RecentReportFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public DateTime dateModified { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the sub module category id.
        /// </summary>
        [DataMember]
        public int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember]
        public int? type { get; set; }

        #endregion
    }
}