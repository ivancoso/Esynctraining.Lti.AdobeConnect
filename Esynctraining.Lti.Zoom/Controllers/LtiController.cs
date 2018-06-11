using System;
using System.Diagnostics.CodeAnalysis;
using Esynctraining.Lti.Zoom.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace Esynctraining.Lti.Zoom.Controllers
{
    public partial class LtiController : BaseController
    {

        #region Constructors and Destructors

        public LtiController(ILogger logger) : base(logger)
        {
            
        }

        #endregion

        public virtual async Task<ActionResult> Home()
        {
            ViewData["Message"] = "Your application description page.";
            return this.View("About");
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> AuthenticationCallback(
            // ReSharper disable once InconsistentNaming
            string __provider__,
            // ReSharper disable once InconsistentNaming
            string __sid__ = null,
            string code = null,
            string state = null,
            string session = null)
        {
            ViewData["Message"] = __provider__;
            return this.View("About");
        }

        public virtual async Task<ActionResult> LoginWithProvider(string provider, LtiParamDTO param)
        {
            ViewData["Message"] = provider;
            return this.View("About");
        }
    }
}
