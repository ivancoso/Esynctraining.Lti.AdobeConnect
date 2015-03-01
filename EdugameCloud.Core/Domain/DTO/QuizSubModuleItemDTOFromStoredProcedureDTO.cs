// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuizSubModuleItemDTOFromStoredProcedureDTO.cs" company="">
//   
// </copyright>
// <summary>
//   The sub module item DTO.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Extensions;

    /// <summary>
    ///     The sub module item DTO.
    /// </summary>
    [DataContract]
    public class QuizSubModuleItemDTOFromStoredProcedureDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizSubModuleItemDTOFromStoredProcedureDTO"/> class.
        /// </summary>
        public QuizSubModuleItemDTOFromStoredProcedureDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizSubModuleItemDTOFromStoredProcedureDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public QuizSubModuleItemDTOFromStoredProcedureDTO(QuizSubModuleItemDTOFromStoredProcedureExDTO dto)
        {
            this.createdBy = dto.createdBy;
            this.dateCreated = dto.dateCreated.ConvertToUnixTimestamp();
            this.dateModified = dto.dateModified.ConvertToUnixTimestamp();
            this.isActive = dto.isActive;
            this.isShared = dto.isShared;
            this.modifiedBy = dto.modifiedBy;
            this.subModuleCategoryId = dto.subModuleCategoryId;
            this.subModuleId = dto.subModuleId;
            this.subModuleItemId = dto.subModuleItemId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? createdBy { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is shared.
        /// </summary>
        [DataMember]
        public bool? isShared { get; set; }

        /// <summary>
        ///     Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int? modifiedBy { get; set; }

        /// <summary>
        ///     Gets or sets the sub module category.
        /// </summary>
        [DataMember]
        public int subModuleCategoryId { get; set; }

        /// <summary>
        ///     Gets or sets the sub module id.
        /// </summary>
        [DataMember]
        public int subModuleId { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        #endregion
    }
}