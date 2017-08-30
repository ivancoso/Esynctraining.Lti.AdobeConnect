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
        public HomeViewModel()
        {
            IP = HttpContext.Current.GetIPAddress();
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

    public static class HttpContextExtensions
    {
        #region Public Methods and Operators

        public static string GetIPAddress()
        {
            HttpContext context = HttpContext.Current;
            return context.GetIPAddress();
        }

        public static string GetIPAddress(this HttpContext context)
        {
            string address = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(address))
            {
                string[] addresses = address.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        #endregion
    }

}