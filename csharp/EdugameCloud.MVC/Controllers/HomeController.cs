using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.UI;
using EdugameCloud.Core;
using EdugameCloud.Core.Business;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.MVC.Attributes;
using EdugameCloud.MVC.ViewModels;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.MVC.Controllers
{
    [HandleError]
    [NonDebugModeRequireHttps]
    public partial class HomeController : BaseController
    {
        private readonly UserActivationModel _userActivationModel;
        private readonly UserModel _userModel;
        private readonly IBuildVersionProcessor _versionProcessor;

        private ICache Cache => IoC.Resolve<ICache>(CachePolicies.Names.PersistantCache);


        public HomeController(UserModel userModel, UserActivationModel userActivationModel, ApplicationSettingsProvider settings, IBuildVersionProcessor versionProcessor)
            : base(settings)
        {
            _userModel = userModel ?? throw new ArgumentNullException(nameof(userModel));
            _userActivationModel = userActivationModel ?? throw new ArgumentNullException(nameof(userActivationModel));
            _versionProcessor = versionProcessor ?? throw new ArgumentNullException(nameof(versionProcessor));
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
                return RedirectToAction("callback", "Lti", new { __provider__,  __sid__, code, state, providerKey });
            }
            else
            {
                var filePattern = (string) Settings.BuildSelector;
                var path = Server.MapPath("~/Content/swf/admin");
                var version = CacheUtility.GetCachedItem(Cache, CachePolicies.Keys.VersionFileName(filePattern), () =>
                    _versionProcessor.ProcessVersion(path, filePattern));
                var versionFileSwf = filePattern.Replace("*", version.ToString());
                return View(
                    EdugameCloudT4.Home.Views.Admin,
                    new HomeViewModel(this) { BuildUrl = Links.Content.swf.admin.Url(versionFileSwf) });
            }
        }
                
        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public virtual ActionResult Activate(string code)
        {
            var passwordActivation = _userActivationModel.GetOneByCode(code).Value;
            var contact = passwordActivation.With(x => x.User);
            if (contact != null)
            {
                contact.Status = UserStatus.Activating;
                _userModel.RegisterSave(contact, true);
                return RedirectToAction(EdugameCloudT4.Home.Admin("activate", code));
            }

            return RedirectToAction(EdugameCloudT4.Home.Admin());
        }
        
        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public virtual ActionResult Config(string referer)
        {
            if (referer == "EGC")
            {
                return Content(Convert.ToBase64String(System.IO.File.ReadAllBytes(Server.MapPath(Links.Content.swf.config.paths_properties))));
            }

            return RedirectToAction(EdugameCloudT4.Home.Admin());
        }
        
        #endregion
        
    }

}