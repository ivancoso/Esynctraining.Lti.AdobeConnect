namespace EdugameCloud.Core.Domain.DTO
{
    /// <summary>
    /// The lms authentication parameters dto
    /// </summary>
    public class LmsAuthenticationParametersDTO
    {
        /// <summary>
        /// Gets or sets the ac id.
        /// </summary>
        public string acId { get; set; }

        /// <summary>
        /// Gets or sets the ac domain.
        /// </summary>
        public string acDomain { get; set; }

        /// <summary>
        /// Gets or sets the meeting sco id.
        /// </summary>
        public string meetingScoId { get; set; }
    }
}
