namespace EdugameCloud.MVC.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    using EdugameCloud.MVC.Attributes;

    using Esynctraining.Core.Extensions;
    using BaseController = EdugameCloud.MVC.Controllers.BaseController;

    /// <summary>
    /// The login view model.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="baseController">
        /// The base controller.
        /// </param>
        public LoginViewModel(BaseController baseController)
            : base(baseController)
        {
            this.LoginAttemptFailed = !this.ModelState.With(x => x.IsValid);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        public LoginViewModel()
        {
            this.LoginAttemptFailed = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether login attempt failed.
        /// </summary>
        public bool LoginAttemptFailed { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataType(DataType.Password)]
        [LocalizedRequired("PasswordRequired", "LogIn")]
        [LocalizedDisplayName("Password", ResourceName = "LogIn")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember me.
        /// </summary>
        [LocalizedDisplayName("RememberMe", ResourceName = "LogIn")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the return url.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [LocalizedDisplayName("UserName", ResourceName = "LogIn")]
        [LocalizedRequired("UserNameRequired", "LogIn")]
        public string UserName { get; set; }

        #endregion
    }
}
