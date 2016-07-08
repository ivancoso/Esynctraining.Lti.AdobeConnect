namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using System.Web.UI;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.MVC.ViewModels;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    [HandleError]
    public partial class HomeController : BaseController
    {

        private readonly UserActivationModel userActivationModel;
        private readonly UserModel userModel;


        public HomeController(UserModel userModel, UserActivationModel userActivationModel, ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.userModel = userModel;
            this.userActivationModel = userActivationModel;
        }
        

        #region Public Methods and Operators
        
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed. Suppression is OK here."), HttpGet]
        public virtual ActionResult Admin(string view = null, string code = null, string __provider__ = null, string __sid__ = null, string state = null, string providerKey = null)
        {
            if (__provider__ == "canvas" 
                && !string.IsNullOrWhiteSpace(__sid__) 
                && !string.IsNullOrWhiteSpace(code)
                && !string.IsNullOrWhiteSpace(state))
            {
                //// crazy hack for canvas OAuth callback ??!!
                return this.RedirectToAction("callback", "Lti", new { __provider__,  __sid__, code, state, providerKey });
            }
            else
            {
                string versionFileSwf = this.ProcessVersion("~/Content/swf/admin", (string)this.Settings.BuildSelector);
                return this.View(
                    EdugameCloudT4.Home.Views.Admin,
                    new HomeViewModel(this) { BuildUrl = Links.Content.swf.admin.Url(versionFileSwf) });
            }
        }
                
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
        
        #endregion
        
    }

}