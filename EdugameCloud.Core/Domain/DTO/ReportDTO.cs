// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportDTO.cs" company="">
//   
// </copyright>
// <summary>
//   The report DTO.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Extensions;

    /// <summary>
    ///     The report DTO.
    /// </summary>
    [DataContract]
    public class ReportDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDTO"/> class.
        /// </summary>
        public ReportDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public ReportDTO(ReportFromStoredProcedureDTO dto)
        {
            this.dateCreated = dto.dateCreated.ConvertToUnixTimestamp();
            this.acSessionId = dto.acSessionId;
            this.type = dto.type;
            this.subModuleItemId = dto.subModuleItemId;
            this.name = dto.name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int acSessionId { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        ///     Gets or sets the sub Module Item Id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [DataMember]
        public int? type { get; set; }

        #endregion
    }
}