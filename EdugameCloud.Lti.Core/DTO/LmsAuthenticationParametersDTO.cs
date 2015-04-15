namespace EdugameCloud.Lti.DTO
{
    /// <summary>
    /// The LMS authentication parameters DTO
    /// </summary>
    public class LmsAuthenticationParametersDTO
    {
        /// <summary>
        /// Gets or sets the AC id.
        /// </summary>
        public string acId { get; set; }

        /// <summary>
        /// Gets or sets the AC domain.
        /// </summary>
        public string acDomain { get; set; }

        /// <summary>
        /// Gets or sets the meeting SCO Id.
        /// </summary>
        public string meetingScoId { get; set; }
    }
}
