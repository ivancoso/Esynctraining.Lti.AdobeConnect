namespace EdugameCloud.Lti.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The proxy tool password model.
    /// </summary>
    public class ProxyToolPasswordModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the black board title.
        /// </summary>
        public string BlackBoardTitle { get; set; }

        /// <summary>
        /// Gets or sets the LMS domain.
        /// </summary>
        public string LmsDomain { get; set; }

        /// <summary>
        /// Gets or sets the registration password.
        /// </summary>
        [Display(Name = "Registration Password:")]
        [Required(ErrorMessage = "Name is Requirde")]
        public string RegistrationPassword { get; set; }

        public string LtiVersion { get; set; }

        #endregion
    }
}