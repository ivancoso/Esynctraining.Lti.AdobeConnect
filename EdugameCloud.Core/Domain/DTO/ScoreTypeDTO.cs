// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScoreTypeDTO.cs" company="">
//   
// </copyright>
// <summary>
//   The score type dto.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The score type dto.
    /// </summary>
    [DataContract]
    public class ScoreTypeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreTypeDTO"/> class.
        /// </summary>
        public ScoreTypeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreTypeDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public ScoreTypeDTO(ScoreType result)
        {
            if (result != null)
            {
                this.scoreTypeId = result.Id;
                this.scoreTypeName = result.ScoreTypeName;
                this.dateCreated = result.DateCreated;
                this.isActive = result.IsActive;
                this.defaultValue = result.DefaultValue;
                this.prefix = result.Prefix;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the default value.
        /// </summary>
        [DataMember]
        public int defaultValue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        ///     Gets or sets the prefix.
        /// </summary>
        [DataMember]
        public string prefix { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        public int scoreTypeId { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        [DataMember]
        public string scoreTypeName { get; set; }

        #endregion
    }
}