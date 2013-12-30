namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The build version DTO.
    /// </summary>
    [DataContract]
    public class BuildVersionDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildVersionDTO" /> class.
        /// </summary>
        public BuildVersionDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildVersionDTO"/> class.
        /// </summary>
        /// <param name="build">
        /// The build.
        /// </param>
        public BuildVersionDTO(BuildVersion build)
        {
            this.buildVersionTypeId = build.Type.Id;
            this.buildVersionType = build.Type.BuildVersionTypeName;
            this.buildVersionId = build.Id;
            this.buildNumber = build.BuildNumber;
            this.dateCreated = build.DateCreated;
            this.dateModified = build.DateModified;
            this.descriptionHTML = build.DescriptionHTML;
            this.descriptionSmall = build.DescriptionSmall;
            this.isActive = build.IsActive;
            this.fileId = build.File.Id;
            this.fileName = build.File.FileName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the build number.
        /// </summary>
        [DataMember]
        public virtual string buildNumber { get; set; }

        /// <summary>
        ///     Gets or sets the version id.
        /// </summary>
        [DataMember]
        public virtual int buildVersionId { get; set; }

        /// <summary>
        ///     Gets or sets the type name.
        /// </summary>
        [DataMember]
        public virtual string buildVersionType { get; set; }

        /// <summary>
        ///     Gets or sets the type id.
        /// </summary>
        [DataMember]
        public virtual int buildVersionTypeId { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public virtual DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public virtual DateTime dateModified { get; set; }

        /// <summary>
        ///     Gets or sets the description html.
        /// </summary>
        [DataMember]
        public virtual string descriptionHTML { get; set; }

        /// <summary>
        ///     Gets or sets the description small.
        /// </summary>
        [DataMember]
        public virtual string descriptionSmall { get; set; }

        /// <summary>
        ///     Gets or sets the file id.
        /// </summary>
        [DataMember]
        public int fileId { get; set; }

        /// <summary>
        ///     Gets or sets the file name.
        /// </summary>
        [DataMember]
        public string fileName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public virtual bool isActive { get; set; }

        #endregion
    }
}