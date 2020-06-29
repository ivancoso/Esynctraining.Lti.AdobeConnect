namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     The user role.
    /// </summary>
    [DataContract]
    public class VCFProfileDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the xml.
        /// </summary>
        [DataMember]
        public string xmlProfile { get; set; }

        #endregion
    }
}