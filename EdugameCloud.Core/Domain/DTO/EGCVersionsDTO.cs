namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The versions DTO.
    /// </summary>
    [DataContract]
    // ReSharper disable once InconsistentNaming
    public class EGCVersionsDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the admin.
        /// </summary>
        [DataMember]
        public VersionDTO adminVersion { get; set; }

        /// <summary>
        /// Gets or sets the public.
        /// </summary>
        [DataMember]
        public VersionDTO publicVersion { get; set; }

        #endregion
    }
}