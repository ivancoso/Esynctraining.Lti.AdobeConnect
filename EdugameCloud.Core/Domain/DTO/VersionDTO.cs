namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The version DTO.
    /// </summary>
    [DataContract]
    public class VersionDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionDTO"/> class.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        public VersionDTO(Version version)
        {
            this.versionString = version.ToString();
            this.major = version.Major;
            this.minor = version.Minor;
            this.build = version.Build;
            this.revision = version.Revision;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionDTO"/> class.
        /// </summary>
        public VersionDTO()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        [DataMember]
        public int build { get; set; }

        /// <summary>
        /// Gets or sets the major.
        /// </summary>
        [DataMember]
        public int major { get; set; }

        /// <summary>
        /// Gets or sets the minor.
        /// </summary>
        [DataMember]
        public int minor { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        [DataMember]
        public int revision { get; set; }

        /// <summary>
        /// Gets or sets the version string.
        /// </summary>
        [DataMember]
        public string versionString { get; set; }

        #endregion
    }
}