namespace EdugameCloud.MVC.ViewModels
{
    using System.Web;
    using Esynctraining.Core.Extensions;
    using BaseController = EdugameCloud.MVC.Controllers.BaseController;

    /// <summary>
    /// The home view model.
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public HomeViewModel(BaseController controller)
            : base(controller)
        {
            this.IP = HttpContext.Current.GetIPAddress();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        public HomeViewModel(BaseController controller, int? page)
            : base(controller, page)
        {
            this.IP = HttpContext.Current.GetIPAddress();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        public HomeViewModel()
        {
            this.IP = HttpContext.Current.GetIPAddress();
        }

        #endregion

        /// <summary>
        /// Gets or sets the build url.
        /// </summary>
        public string BuildUrl { get; set; }

        /// <summary>
        /// Gets or sets the IP.
        /// </summary>
        public string IP { get; set; }
    }
}