namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Web.UI;

    using Castle.Core.Logging;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.MVC.Resources;
    using EdugameCloud.MVC.ViewModels;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    /// <summary>
    ///     The home controller.
    /// </summary>
    [HandleError]
    public partial class HomeController : BaseController
    {
        #region Fields

        /// <summary>
        ///     The password activation model.
        /// </summary>
        private readonly UserActivationModel userActivationModel;

        /// <summary>
        ///     The contact model.
        /// </summary>
        private readonly UserModel userModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="userModel">
        /// The contact Model.
        /// </param>
        /// <param name="userActivationModel">
        /// The password Activation Model.
        /// </param>
        /// <param name="settings">
        /// The settings
        /// </param>
        public HomeController(UserModel userModel, UserActivationModel userActivationModel, ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.userModel = userModel;
            this.userActivationModel = userActivationModel;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult Admin(string view = null, string code = null)
        {
            string versionFileSwf = this.ProcessVersion("~/Content/swf/admin", (string)this.Settings.BuildSelector);
            return this.View(EdugameCloudT4.Home.Views.Admin, new HomeViewModel(this) { BuildUrl = Links.Content.swf.admin.Url(versionFileSwf) });
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult Cookies()
        {
            var sb = new StringBuilder("Server side cookies:");
            foreach (string cookie in this.Request.Cookies)
            {
                sb.AppendLine();
                var c = this.Request.Cookies[cookie];
                if (c != null)
                {
                    sb.AppendFormat(
                        "{0}: http: {1}, path: {2}, value: {3}, expires: {4}",
                        c.Name,
                        c.HttpOnly,
                        c.Path,
                        c.Value,
                        c.Expires);
                }
            }

            return this.Content(sb.ToString());
        }

        /// <summary>
        ///     The about.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public virtual ActionResult LogIn()
        {
            return this.View(new LoginViewModel(this));
        }

        /// <summary>
        /// The log on.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult LogIn(LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = this.userModel.GetOneByEmail(model.UserName.Trim()).Value;

                if (user != null && user.Status == UserStatus.Active && user.ValidatePassword(model.Password.Trim()))
                {
                    return this.RedirectFromLoginPage(user, model.ReturnUrl, model.RememberMe);
                }

                this.ModelState.AddModelError(Lambda.Property<LoginViewModel>(x => x.Password), WebResources.LogIn.LoginFailed);
            }

            return this.View(model);
        }

        /// <summary>
        /// The development.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult Activate(string code)
        {
            var passwordActivation = this.userActivationModel.GetOneByCode(code).Value;
            var contact = passwordActivation.With(x => x.User);
            if (contact != null)
            {
                contact.Status = UserStatus.Activating;
                this.userModel.RegisterSave(contact, true);
                return this.RedirectToAction(EdugameCloudT4.Home.Admin("activate", code));
            }

            return this.RedirectToAction(EdugameCloudT4.Home.Admin());
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="referer">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public virtual ActionResult Config(string referer)
        {
            if (referer == "EGC")
            {
                return this.Content(Convert.ToBase64String(System.IO.File.ReadAllBytes(this.Server.MapPath(Links.Content.swf.config.paths_properties))));
            }

            return this.RedirectToAction(EdugameCloudT4.Home.Admin());
        }

        /// <summary>
        ///     The config.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        [HttpGet]
        public virtual ActionResult Config()
        {
            return this.RedirectToAction(EdugameCloudT4.Home.Admin());
        }

        /// <summary>
        ///     The history.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        [HttpGet]
        public virtual ActionResult History()
        {
            return this.View(new HistoryViewModel(this));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The redirect from login page.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="createPersistentCookie">
        /// The create persistent cookie.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [NonAction]
        protected ActionResult RedirectFromLoginPage(User user, string returnUrl, bool createPersistentCookie)
        {
            try
            {
                FormsAuthentication.RedirectFromLoginPage(user.Email, createPersistentCookie);
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error("cookie", ex);
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.Redirect(AuthenticationModel.DefaultUrl);
        }

        #endregion
    }
}