namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;

    /// <summary>
    /// The Black Board consumer profile DTO.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class BBConsumerProfileDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the LTI Version.
        /// </summary>
        public string LtiVersion { get; set; }

        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        public List<string> Services { get; set; }

        #endregion
    }
}