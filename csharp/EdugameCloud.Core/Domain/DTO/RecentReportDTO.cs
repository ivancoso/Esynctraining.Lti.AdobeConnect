// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentReportDTO.cs" company="">
//   
// </copyright>
// <summary>
//   The recent report DTO.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The recent report DTO.
    /// </summary>
    [DataContract]
    public class RecentReportDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentReportDTO"/> class.
        /// </summary>
        public RecentReportDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentReportDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public RecentReportDTO(RecentReportFromStoredProcedureDTO dto)
        {
            this.dateModified = dto.dateModified.ConvertToUnixTimestamp();
            this.name = dto.name;
            this.subModuleCategoryId = dto.subModuleCategoryId;
            this.subModuleItemId = dto.subModuleItemId;
            this.type = dto.type;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

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