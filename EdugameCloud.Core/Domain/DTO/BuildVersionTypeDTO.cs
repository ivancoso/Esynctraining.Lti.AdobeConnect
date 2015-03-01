namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The build version type DTO.
    /// </summary>
    [DataContract]
    public class BuildVersionTypeDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildVersionTypeDTO"/> class.
        /// </summary>
        public BuildVersionTypeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildVersionTypeDTO"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public BuildVersionTypeDTO(BuildVersionType type)
        {
            this.buildVersionTypeId = type.Id;
            this.buildVersionType = type.BuildVersionTypeName;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the build version type.
        /// </summary>
        [DataMember]
        public string buildVersionType { get; set; }

        /// <summary>
        /// Gets or sets the build version type id.
        /// </summary>
        [DataMember]
        public int buildVersionTypeId { get; set; }

        #endregion
    }
}